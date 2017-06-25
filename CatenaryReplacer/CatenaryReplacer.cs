using System.Collections;
using ICities;
using System.Collections.Generic;
using CatenaryReplacer.OptionsFramework;
using CatenaryReplacer.OptionsFramework.Extensions;
using UnityEngine;

namespace CatenaryReplacer
{
    public class CatenaryReplacerMod : LoadingExtensionBase, IUserMod
    {
        private string[] styles = new string[] { "No Catenary", "Dutch A", "Dutch B", "German A", "PRR A", "PRR B", "Japan A" };

        private GameObject go;

        /// <summary>
        /// Saves the changed lane prop state;
        /// </summary>
        private class ReplacementState
        {
            public NetInfo prefab;

            public int laneIndex;
            public int propIndex;

            public PropInfo originalProp;
            public PropInfo replacementProp;
        }

        private static readonly List<ReplacementState> changes = new List<ReplacementState>();

        public string Name
        {
            get { return "Catenary Replacer"; }
        }

        public string Description
        {
            get { return "Replaces the catenaries on railroads with those of your choice"; }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<CatenaryReplacerConfiguration>();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            changes.Clear();

            var config = OptionsWrapper<CatenaryReplacerConfiguration>.Options;


            var configStyle = (CatenaryStyle)config.Style;
            switch (configStyle)
            {
                case CatenaryStyle.None:
                    ReplaceCatenaries(null, null);
                    RemoveWires();
                    break;
                case CatenaryStyle.DutchTypeA:
                    ReplaceCatenaries("774449380.Catenary Type NL2A_Data", "774449380.Catenary Type NL1A_Data");
                    break;
                case CatenaryStyle.DutchTypeB:
                    ReplaceCatenaries("774449380.Catenary Type NL2B_Data", "774449380.Catenary Type NL1B_Data");
                    break;
                case CatenaryStyle.German:
                    ReplaceCatenaries("774449380.Catenary Type DE2A_Data", "774449380.Catenary Type DE1A_Data");
                    break;
                case CatenaryStyle.PrrA:
                    ReplaceCatenaries("774449380.Catenary Type PRR2A_Data", "774449380.Catenary Type PRR 1A_Data");
                    break;
                case CatenaryStyle.PrrB:
                    ReplaceCatenaries("774449380.Catenary Type PRR2B_Data", "774449380.Catenary Type PRR 1A_Data");
                    break;
                case CatenaryStyle.JapanA:
                    //Japan A
                    ReplaceCatenaries("774449380.Catenary Type JP2A_Data", "774449380.Catenary Type JP1A_Data");
                    break;
            }
        }
        public override void OnLevelUnloading()
        {
            RevertLaneProps();

            if (go != null) Object.Destroy(go);
        }

        private void ReplaceCatenaries(string doubleReplacement, string singleReplacement)
        {
            go = new GameObject("CatenaryReplacer");
            var replacer = go.AddComponent<Replacer>();
            replacer.doubleReplacement = doubleReplacement;
            replacer.singleReplacement = singleReplacement;
        }

        private class Replacer : MonoBehaviour
        {
            public string doubleReplacement;
            public string singleReplacement;

            public void Awake()
            {
                StartCoroutine(ExecuteAfterTime());
            }

            private IEnumerator ExecuteAfterTime()
            {
                yield return new WaitForSeconds(0.1f);

                ReplaceLaneProp("Train Track", "RailwayPowerline", doubleReplacement);
                ReplaceLaneProp("Train Cargo Track", "RailwayPowerline", doubleReplacement);
                ReplaceLaneProp("Train Track Bridge", "RailwayPowerline", doubleReplacement);
                ReplaceLaneProp("Train Track Elevated", "RailwayPowerline", doubleReplacement);
                ReplaceLaneProp("Train Track Tunnel", "RailwayPowerline", doubleReplacement);
                ReplaceLaneProp("Train Cargo Track Elevated", "RailwayPowerline", doubleReplacement);
                ReplaceLaneProp("Oneway Train Track", "RailwayPowerline", doubleReplacement);
                ReplaceLaneProp("Station Track Sunken", "RailwayPowerline", doubleReplacement);
                ReplaceLaneProp("Train Station Track (C)", "RailwayPowerline", doubleReplacement);
                ReplaceLaneProp("Station Track Elevated Narrow (C)", "RailwayPowerline", doubleReplacement);
                ReplaceLaneProp("Station Track Eleva", "RailwayPowerline", doubleReplacement);
                ReplaceLaneProp("Station Track Elevated (C)", "RailwayPowerline", doubleReplacement);
                ReplaceLaneProp("Station Track Elevated Narrow", "RailwayPowerline", doubleReplacement);

                ReplaceLaneProp("Rail1L", "724382534.Rail1LPowerLine_Data", singleReplacement);
                ReplaceLaneProp("Rail1L Slope", "RailwayPowerline", singleReplacement); //Depends on One-Way Tracks update
                ReplaceLaneProp("Rail1L Elevated", "724382534.Rail1LPowerLine_Data", singleReplacement);
                ReplaceLaneProp("Rail1L Bridge", "724382534.Rail1LPowerLine_Data", singleReplacement);
                ReplaceLaneProp("Rail1L Tunnel", "724382534.Rail1LPowerLine_Data", singleReplacement);
                ReplaceLaneProp("Rail1L2W", "724382534.Rail1LPowerLine_Data", singleReplacement);
                ReplaceLaneProp("Rail1L2W Slope", "RailwayPowerline", singleReplacement); //Depends on One-Way Tracks update
                ReplaceLaneProp("Rail1L2W Elevated", "724382534.Rail1LPowerLine_Data", singleReplacement);
                ReplaceLaneProp("Rail1L2W Bridge", "724382534.Rail1LPowerLine_Data", singleReplacement);
                ReplaceLaneProp("Rail1L2W Tunnel", "724382534.Rail1LPowerLine_Data", singleReplacement);
                ReplaceLaneProp("Rail1L2SidedStation", "724382534.Rail1LPowerLine_Data", singleReplacement);
                ReplaceLaneProp("Rail1LStation", "724382534.Rail1LPowerLine_Data", singleReplacement);

                Destroy(this.gameObject);
            }
        }

        private void RemoveWires()
        {
            //Segments
            RemoveSegment("Train Track", 2);
            RemoveSegment("Train Cargo Track", 2);
            RemoveSegment("Train Track Bridge", 2);
            RemoveSegment("Train Track Elevated", 2);
            RemoveSegment("Train Track Slope", 2);
            RemoveSegment("Train Cargo Track Elevated", 2);
            RemoveSegment("Oneway Train Track", 2);
            RemoveSegment("Oneway Train Track Elevated", 2);
            RemoveSegment("Oneway Train Track Slope", 2);
            RemoveSegment("Oneway Train Track Bridge", 2);
            RemoveSegment("Station Track Sunken", 2);
            RemoveSegment("Train Station Track (C)", 2);
            RemoveSegment("Train Station Track", 2);
            RemoveSegment("Station Track Elevated Narrow (C)", 2);
            RemoveSegment("Station Track Eleva", 2);
            RemoveSegment("Station Track Elevated (C)", 2);
            RemoveSegment("Station Track Elevated Narrow", 2);

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

            //Nodes
            RemoveNode("Train Track", 3);
            RemoveNode("Train Cargo Track", 3);
            RemoveNode("Train Track Bridge", 2);
            RemoveNode("Train Track Elevated", 2);
            RemoveNode("Train Cargo Track Elevated", 3);
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
            RemoveNode("Station Track Sunken", 3);
            RemoveNode("Train Station Track (C)", 3);
            RemoveNode("Train Station Track", 3);
            RemoveNode("Station Track Elevated Narrow (C)", 3);
            RemoveNode("Station Track Eleva", 3);
            RemoveNode("Station Track Elevated (C)", 3);
            RemoveNode("Station Track Elevated Narrow", 3);

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
        }

        private static void RemoveSegment(string net, int segment)
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

            netInfo.m_segments[segment].m_segmentMesh = null;
            netInfo.m_segments[segment].m_lodMesh = null;
        }
        private static void RemoveNode(string net, int node)
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

            netInfo.m_nodes[node].m_nodeMesh = null;
            netInfo.m_nodes[node].m_lodMesh = null;
        }
        private static void ReplaceLaneProp(string net, string original, string replacement)
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
                                changes.Add(new ReplacementState
                                {
                                    prefab = netInfo,
                                    laneIndex = laneIndex,
                                    propIndex = propIndex,
                                    originalProp = laneProp.m_prop,
                                    replacementProp = replacementProp
                                });

                                laneProp.m_prop = replacementProp;
                                laneProp.m_finalProp = replacementProp;
                            }
                        }
                    }
                }
            }
        }

        private void RevertLaneProps()
        {
            foreach (var state in changes)
            {
                var laneProp = state.prefab.m_lanes[state.laneIndex].m_laneProps.m_props[state.propIndex];

                laneProp.m_prop = state.originalProp;
                laneProp.m_finalProp = state.originalProp;
            }
            changes.Clear();
        }
    }


}