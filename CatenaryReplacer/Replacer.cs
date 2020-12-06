﻿using System.Collections;
using System.Collections.Generic;
using CatenaryReplacer.OptionsFramework;
using UnityEngine;

namespace CatenaryReplacer
{
    //TODO(earalov): use more generic approach for replacing props
    //TODO(earalov): add support for more track types
    //TODO(earalov): add offset for props when updating vanilla one-ways
    //TODO(earalov): implement no-wire for vanilla one-ways

    //DONE(Tim): Attempt to support Jerenable's tracks (temporary?)

    public class Replacer : MonoBehaviour
    {
        /// <summary>
        /// Saves the changed lane prop state;
        /// </summary>
        private struct ReplacementStateProp
        {
            public NetInfo prefab;

            public int laneIndex;
            public int propIndex;

            public PropInfo originalProp;
            public PropInfo replacementProp;

            public float originalAngle;
        }

        private struct ReplacementStateSegment
        {
            public string netInfoName;
            public Mesh mesh;
            public Mesh lodMesh;
            public NetInfo.LodValue combinedLod;
            public int index;
        }



        private readonly List<ReplacementStateProp> propChanges = new List<ReplacementStateProp>();
        private readonly List<ReplacementStateSegment> nodeChanges = new List<ReplacementStateSegment>();
        private readonly List<ReplacementStateSegment> segmentChanges = new List<ReplacementStateSegment>();

        public string doubleReplacement;
        public string singleReplacement;

        public void Awake()
        {
            var config = OptionsWrapper<CatenaryReplacerConfiguration>.Options;
            var configStyle = (CatenaryStyle)config.Style;
            switch (configStyle)
            {
                case CatenaryStyle.None:
                    SetReplacementPropNames(null, null);
                    RemoveWires();
                    break;
                case CatenaryStyle.DutchTypeA:
                    SetReplacementPropNames("Catenary Type NL2A", "Catenary Type NL1A");
                    break;
                case CatenaryStyle.DutchTypeB:
                    SetReplacementPropNames("Catenary Type NL2B", "Catenary Type NL1B");
                    break;
                case CatenaryStyle.German:
                    SetReplacementPropNames("Catenary Type DE2A", "Catenary Type DE1A");
                    break;
                case CatenaryStyle.PrrA:
                    SetReplacementPropNames("Catenary Type PRR2A", "Catenary Type PRR 1A");
                    break;
                case CatenaryStyle.PrrB:
                    SetReplacementPropNames("Catenary Type PRR2B", "Catenary Type PRR 1A");
                    break;
                case CatenaryStyle.JapanA:
                    SetReplacementPropNames("Catenary Type JP2A", "Catenary Type JP1A");
                    break;
                case CatenaryStyle.ExpoA:
                    SetReplacementPropNames("Catenary Type EXPO2A", "Catenary Type EXPO1A");
                    break;
            }
            
        }

        public void Start()
        {
            StartCoroutine(ExecuteAfterTime());
        }

        public void OnDestroy()
        {
            RevertLaneProps();
            RevertNodes();
            RevertSegments();
        }

        private IEnumerator ExecuteAfterTime()
        {
            yield return new WaitForSeconds(0.1f);
            //Vanilla
            ReplaceLaneProp("Train Track", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("Train Cargo Track", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("Train Track Bridge", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("Train Track Elevated", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("Train Track Tunnel", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("Train Cargo Track Elevated", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("Oneway Train Track", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("Station Track Sunken", "RailwayPowerline", doubleReplacement);
            //Extra Station Tracks (Mod)
            ReplaceLaneProp("Train Station Track (C)", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("Station Track Elevated Narrow (C)", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("Station Track Eleva", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("Station Track Elevated (C)", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("Station Track Elevated Narrow", "RailwayPowerline", doubleReplacement);
            //Better Rail by Jerenable
            ReplaceLaneProp("1178802767.Better Vanilla Rail_Data", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("1178802767.Train Track Bridge0", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("1178802767.Train Track Elevated0", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("1178802767.Train Track Slope0", "RailwayPowerline", doubleReplacement);
            //Highspeed Rail by Jerenable
            ReplaceLaneProp("1221219565.Highspeed Rail vanilla style_Data", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("1221219565.Train Track Bridge0", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("1221219565.Train Track Elevated0", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("1221219565.Train Track Slope0", "RailwayPowerline", doubleReplacement);
            //OneWay Tracks (Mod)
            ReplaceLaneProp("Rail1L", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Rail1L Slope", "RailwayPowerline", singleReplacement); //Depends on One-Way Tracks update
            ReplaceLaneProp("Rail1L Elevated", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Rail1L Bridge", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Rail1L Tunnel", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Rail1L2W", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Rail1L2W Slope", "RailwayPowerline", singleReplacement); //Depends on One-Way Tracks update
            ReplaceLaneProp("Rail1L2W Elevated", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Rail1L2W Bridge", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Rail1L2W Tunnel", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Rail1L2SidedStation", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Rail1LStation", "RailwayPowerline Singular", singleReplacement);
            //OneWay Tracks (Vanilla)
            ReplaceLaneProp("Train Oneway Track", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Train Oneway Track Slope", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Train Oneway Track Bridge", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Train Oneway Track Elevated", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Train Oneway Track Tunnel", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Train Oneway Track Tunnel", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("Train Oneway Track Tunnel", "RailwayPowerline Singular", singleReplacement);
            //Cylis' Tracks
            ReplaceLaneProp("2223774659.Rendered Train Tracks_Data", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("2223774659.Train Track Elevated0", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("2223774659.Train Track Bridge0", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("2223774659.Train Track Slope0", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("2223774659.Train Track Tunnel0", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("2223774659.Rendered One-way Train Tracks_Data", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("2223774659.Train Oneway Track Elevated0", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("2223774659.Train Oneway Track Bridge0", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("2223774659.Train Oneway Track Slope0", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("2223774659.Train Oneway Track Tunnel0", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("track_w_bypass_station.Wired Bypass Station_Data", "RailwayPowerline Singular", singleReplacement);
            ReplaceLaneProp("wiredstationtrack.Wired Station Track_Data", "RailwayPowerline", doubleReplacement);
            ReplaceLaneProp("wiredestationtrack.Wired Elevated Station Track_Data", "RailwayPowerline", doubleReplacement);

        }

        private void SetReplacementPropNames(string doubleReplacement, string singleReplacement)
        {
            this.doubleReplacement = doubleReplacement == null ? null : $"774449380.{doubleReplacement}_Data";
            this.singleReplacement = singleReplacement == null ? null : $"774449380.{singleReplacement}_Data";
        }

        private void RemoveWires()
        {
            //Segments

            //Vanilla
            RemoveSegment("Train Track", 2);
            RemoveSegment("Train Cargo Track", 2);
            RemoveSegment("Train Track Bridge", 2);
            RemoveSegment("Train Track Elevated", 2);
            RemoveSegment("Train Track Slope", 2);
            RemoveSegment("Train Cargo Track Elevated", 2);
            //OneWay Tracks (Vanilla)
            RemoveSegment("Oneway Train Track", 2);
            RemoveSegment("Oneway Train Track Elevated", 2);
            RemoveSegment("Oneway Train Track Slope", 2);
            RemoveSegment("Oneway Train Track Bridge", 2);
            RemoveSegment("Station Track Sunken", 2);
            //Extra Station Tracks (Mod)
            RemoveSegment("Train Station Track (C)", 2);
            RemoveSegment("Train Station Track", 2);
            RemoveSegment("Station Track Elevated Narrow (C)", 2);
            RemoveSegment("Station Track Eleva", 2);
            RemoveSegment("Station Track Elevated (C)", 2);
            RemoveSegment("Station Track Elevated Narrow", 2);
            //OneWay Tracks (Mod)
            RemoveSegment("Rail1L", 2);
            RemoveSegment("Rail1L Slope", 2);
            RemoveSegment("Rail1L Elevated", 2);
            RemoveSegment("Rail1L Bridge", 2);
            RemoveSegment("Rail1L2W", 2);
            RemoveSegment("Rail1L2W Slope", 2);
            RemoveSegment("Rail1L2W Elevated", 2);
            RemoveSegment("Rail1L2W Bridge", 2);
            RemoveSegment("Rail1L2SidedStation", 2);
            RemoveSegment("Rail1LStation", 2);
            //Better Rail by Jerenable
            RemoveSegment("1178802767.Better Vanilla Rail_Data", 2);
            RemoveSegment("1178802767.Train Track Bridge0", 2);
            RemoveSegment("1178802767.Train Track Slope0", 2);
            RemoveSegment("1178802767.Train Track Elevated0", 2);
            //Highspeed Rail by Jerenable
            RemoveSegment("1221219565.Highspeed Rail vanilla style_Data", 2);
            RemoveSegment("1221219565.Train Track Bridge0", 2);
            RemoveSegment("1221219565.Train Track Slope0", 2);
            RemoveSegment("1221219565.Train Track Elevated0", 2);
            // Cylis' Tracks
            RemoveSegment("2223774659.Rendered Train Tracks_Data", 4);
            RemoveSegment("2223774659.Train Track Elevated0", 5);
            RemoveSegment("2223774659.Train Track Bridge0", 5);
            RemoveSegment("2223774659.Train Track Slope0", 4);
            RemoveSegment("2223774659.Train Track Tunnel0", 4);
            RemoveSegment("2223774659.Rendered One-way Train Tracks_Data", 4);
            RemoveSegment("2223774659.Train Oneway Track Elevated0", 5);
            RemoveSegment("2223774659.Train Oneway Track Bridge0", 5);
            RemoveSegment("2223774659.Train Oneway Track Slope0", 4);
            RemoveSegment("2223774659.Train Oneway Track Tunnel0", 4);
            RemoveSegment("track_w_bypass_station.Wired Bypass Station_Data", 5);
            RemoveSegment("track_w_bypass_station.Wired Bypass Station_Data", 6);
            RemoveSegment("wiredstationtrack.Wired Station Track_Data", 3);
            RemoveSegment("wiredestationtrack.Wired Elevated Station Track_Data", 4);


            //Nodes

            //Vanilla
            RemoveNode("Train Track", 3);
            RemoveNode("Train Cargo Track", 3);
            RemoveNode("Train Track Bridge", 2);
            RemoveNode("Train Track Elevated", 2);
            RemoveNode("Train Cargo Track Elevated", 3);
            //OneWay Tracks (Vanilla)
            RemoveNode("Oneway Train Track", 3);
            RemoveNode("Oneway Train Track", 6);
            RemoveNode("Oneway Train Track", 7);
            RemoveNode("Oneway Train Track", 10);
            RemoveNode("Oneway Train Track", 11);
            RemoveNode("Oneway Train Track", 13);
            RemoveNode("Oneway Train Track Elevated", 2);
            RemoveNode("Oneway Train Track Elevated", 6);
            RemoveNode("Oneway Train Track Elevated", 7);
            RemoveNode("Oneway Train Track Elevated", 10);
            RemoveNode("Oneway Train Track Elevated", 11);
            RemoveNode("Oneway Train Track Elevated", 13);
            RemoveNode("Oneway Train Track Slope", 2);
            RemoveNode("Oneway Train Track Slope", 7);
            RemoveNode("Oneway Train Track Slope", 8);
            RemoveNode("Oneway Train Track Slope", 11);
            RemoveNode("Oneway Train Track Slope", 12);
            RemoveNode("Oneway Train Track Bridge", 2);
            RemoveNode("Oneway Train Track Bridge", 6);
            RemoveNode("Oneway Train Track Bridge", 7);
            RemoveNode("Oneway Train Track Bridge", 10);
            RemoveNode("Oneway Train Track Bridge", 11);
            RemoveNode("Oneway Train Track Bridge", 13);
            //Extra Station Tracks (Mod)
            RemoveNode("Station Track Sunken", 3);
            RemoveNode("Train Station Track (C)", 3);
            RemoveNode("Train Station Track", 3);
            RemoveNode("Station Track Elevated Narrow (C)", 3);
            RemoveNode("Station Track Eleva", 3);
            RemoveNode("Station Track Elevated (C)", 3);
            RemoveNode("Station Track Elevated Narrow", 3);
            //OneWay Tracks (Mod)
            RemoveNode("Rail1L", 3);
            RemoveNode("Rail1L", 6);
            RemoveNode("Rail1L", 7);
            RemoveNode("Rail1L Slope", 2);
            RemoveNode("Rail1L Slope", 6);
            RemoveNode("Rail1L Slope", 8);
            RemoveNode("Rail1L Elevated", 2);
            RemoveNode("Rail1L Elevated", 6);
            RemoveNode("Rail1L Elevated", 7);
            RemoveNode("Rail1L Bridge", 2);
            RemoveNode("Rail1L Bridge", 6);
            RemoveNode("Rail1L Bridge", 7);
            RemoveNode("Rail1L2W", 3);
            RemoveNode("Rail1L2W", 5);
            RemoveNode("Rail1L2W", 7);
            RemoveNode("Rail1L2W Slope", 2);
            RemoveNode("Rail1L2W Slope", 7);
            RemoveNode("Rail1L2W Slope", 8);
            RemoveNode("Rail1L2W Elevated", 2);
            RemoveNode("Rail1L2W Elevated", 6);
            RemoveNode("Rail1L2W Elevated", 7);
            RemoveNode("Rail1L2W Bridge", 2);
            RemoveNode("Rail1L2W Bridge", 6);
            RemoveNode("Rail1L2W Bridge", 7);
            RemoveNode("Rail1L2SidedStation", 3);
            RemoveNode("Rail1L2SidedStation", 5);
            RemoveNode("Rail1L2SidedStation", 7);
            RemoveNode("Rail1LStation", 3);
            RemoveNode("Rail1LStation", 5);
            RemoveNode("Rail1LStation", 7);
            //Better Rail by Jerenable
            RemoveNode("1178802767.Better Vanilla Rail_Data", 3);
            RemoveNode("1178802767.Train Track Bridge0", 2);
            RemoveNode("1178802767.Train Track Slope0", 2);
            RemoveNode("1178802767.Train Track Elevated0", 2);
            //Highspeed Rail by Jerenable
            RemoveNode("1221219565.Highspeed Rail vanilla style_Data", 3);
            RemoveNode("1221219565.Train Track Bridge0", 2);
            RemoveNode("1221219565.Train Track Slope0", 2);
            RemoveNode("1221219565.Train Track Elevated0", 2);
            // Cylis' Tracks
            RemoveNode("2223774659.Rendered Train Tracks_Data", 4);
            RemoveNode("2223774659.Train Track Elevated0", 6);
            RemoveNode("2223774659.Train Track Bridge0", 4);
            RemoveNode("2223774659.Train Track Slope0", 5);
            RemoveNode("2223774659.Train Track Tunnel0", 3);
            RemoveNode("2223774659.Rendered One-way Train Tracks_Data", 6);
            RemoveNode("2223774659.Rendered One-way Train Tracks_Data", 7);
            RemoveNode("2223774659.Rendered One-way Train Tracks_Data", 8);
            RemoveNode("2223774659.Train Oneway Track Elevated0", 8);
            RemoveNode("2223774659.Train Oneway Track Elevated0", 9);
            RemoveNode("2223774659.Train Oneway Track Elevated0", 10);
            RemoveNode("2223774659.Train Oneway Track Bridge0", 6);
            RemoveNode("2223774659.Train Oneway Track Bridge0", 7);
            RemoveNode("2223774659.Train Oneway Track Bridge0", 8);
            RemoveNode("2223774659.Train Oneway Track Slope0", 8);
            RemoveNode("2223774659.Train Oneway Track Slope0", 9);
            RemoveNode("2223774659.Train Oneway Track Slope0", 10);
            RemoveNode("2223774659.Train Oneway Track Tunnel0", 7);
            RemoveNode("track_w_bypass_station.Wired Bypass Station_Data", 8);
            RemoveNode("track_w_bypass_station.Wired Bypass Station_Data", 9);
            RemoveNode("track_w_bypass_station.Wired Bypass Station_Data", 10);
            RemoveNode("track_w_bypass_station.Wired Bypass Station_Data", 11);
            RemoveNode("track_w_bypass_station.Wired Bypass Station_Data", 12);
            RemoveNode("wiredstationtrack.Wired Station Track_Data", 5);
            RemoveNode("wiredstationtrack.Wired Station Track_Data", 6);
            RemoveNode("wiredestationtrack.Wired Elevated Station Track_Data", 6);
            RemoveNode("wiredestationtrack.Wired Elevated Station Track_Data", 7);

        }

        private void RemoveSegment(string net, int segment)
        {
            var netInfo = PrefabCollection<NetInfo>.FindLoaded(net);
            if (netInfo == null)
            {
                Debug.LogWarning("The name '" + net + "' you entered does not belong to a loaded net!");
                return;
            }

            if (segment >= netInfo.m_segments.Length)
            {
                Debug.LogWarning(net + ": Segment " + segment + " not found!");
                return;
            }

            segmentChanges.Add(new ReplacementStateSegment()
            {
                netInfoName = net,
                index = segment,
                mesh = netInfo.m_segments[segment].m_segmentMesh,
                lodMesh = netInfo.m_segments[segment].m_lodMesh,
                combinedLod = netInfo.m_segments[segment].m_combinedLod
            });

            netInfo.m_segments[segment].m_segmentMesh = null;
            netInfo.m_segments[segment].m_lodMesh = null;
            netInfo.m_segments[segment].m_combinedLod = null;
        }

        private void RemoveNode(string net, int node)
        {
            var netInfo = PrefabCollection<NetInfo>.FindLoaded(net);
            if (netInfo == null)
            {
                Debug.LogWarning("The name '" + net + "' you entered does not belong to a loaded net!");
                return;
            }

            if (node >= netInfo.m_nodes.Length)
            {
                Debug.LogWarning(net + ": Node " + node + " not found!");
                return;
            }

            nodeChanges.Add(new ReplacementStateSegment()
            {
                netInfoName = net,
                index = node,
                mesh = netInfo.m_nodes[node].m_nodeMesh,
                lodMesh = netInfo.m_nodes[node].m_lodMesh,
                combinedLod = netInfo.m_nodes[node].m_combinedLod
            });

            netInfo.m_nodes[node].m_nodeMesh = null;
            netInfo.m_nodes[node].m_lodMesh = null;
            netInfo.m_nodes[node].m_combinedLod = null;
        }

        private void RevertLaneProps()
        {
            foreach (var state in propChanges)
            {
                var laneProp = state.prefab.m_lanes[state.laneIndex].m_laneProps.m_props[state.propIndex];

                laneProp.m_prop = state.originalProp;
                laneProp.m_finalProp = state.originalProp;
                laneProp.m_angle = state.originalAngle;
            }
            propChanges.Clear();
        }

        private void RevertSegments()
        {
            foreach (var state in segmentChanges)
            {
                NetInfo netInfo = PrefabCollection<NetInfo>.FindLoaded(state.netInfoName);
                netInfo.m_segments[state.index].m_segmentMesh = state.mesh;
                netInfo.m_segments[state.index].m_lodMesh = state.lodMesh;
                netInfo.m_segments[state.index].m_combinedLod = state.combinedLod;
            }
            segmentChanges.Clear();
        }

        private void RevertNodes()
        {
            foreach (var state in nodeChanges)
            {
                NetInfo netInfo = PrefabCollection<NetInfo>.FindLoaded(state.netInfoName);
                netInfo.m_nodes[state.index].m_nodeMesh = state.mesh;
                netInfo.m_nodes[state.index].m_lodMesh = state.lodMesh;
                netInfo.m_nodes[state.index].m_combinedLod = state.combinedLod;
            }
            nodeChanges.Clear();
        }

        private void ReplaceLaneProp(string net, string original, string replacement)
        {
            var netInfo = PrefabCollection<NetInfo>.FindLoaded(net);
            if (netInfo == null)
            {
                Debug.LogWarning("The name '" + net + "' you entered does not belong to a loaded net!");
                return;
            }

            var replacementProp = replacement != null ? PrefabCollection<PropInfo>.FindLoaded(replacement) : null;
            if (replacement != null && replacementProp == null)
            {
                Debug.LogWarning("The name '" + replacement + "' you entered does not belong to a loaded prop!");
                return;
            }

            if (netInfo.m_lanes != null)
            {
                for (int laneIndex = 0; laneIndex < netInfo.m_lanes.Length; laneIndex++)
                {
                    var lane = netInfo.m_lanes[laneIndex];

                    if (lane != null && lane.m_laneProps != null && lane.m_laneProps.m_props != null)
                    {
                        for (var propIndex = 0; propIndex < lane.m_laneProps.m_props.Length; propIndex++)
                        {
                            var laneProp = lane.m_laneProps.m_props[propIndex];

                            if (laneProp != null && laneProp.m_prop != null && laneProp.m_prop.name == original)
                            {
                                propChanges.Add(new ReplacementStateProp
                                {
                                    prefab = netInfo,
                                    laneIndex = laneIndex,
                                    propIndex = propIndex,
                                    originalProp = laneProp.m_prop,
                                    replacementProp = replacementProp,
                                    originalAngle = laneProp.m_angle
                                });

                                laneProp.m_prop = replacementProp;
                                laneProp.m_finalProp = replacementProp;
                                laneProp.m_angle = laneProp.m_angle > 180f ? laneProp.m_angle - 180f : laneProp.m_angle + 180f;
                            }
                        }
                    }
                }
            }
        }
    }
}