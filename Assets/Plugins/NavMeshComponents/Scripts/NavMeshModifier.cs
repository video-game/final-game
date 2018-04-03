using System.Collections.Generic;

namespace UnityEngine.AI
{
    [ExecuteInEditMode]
    [AddComponentMenu("Navigation/NavMeshModifier", 32)]
    [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
    public class NavMeshModifier : MonoBehaviour
    {
        [SerializeField]
        bool m_OverrideArea;
#pragma warning disable IDE1006 // Naming Styles
        public bool overrideArea { get { return m_OverrideArea; } set { m_OverrideArea = value; } }
#pragma warning restore IDE1006 // Naming Styles

        [SerializeField]
        int m_Area;
#pragma warning disable IDE1006 // Naming Styles
        public int area { get { return m_Area; } set { m_Area = value; } }
#pragma warning restore IDE1006 // Naming Styles

        [SerializeField]
        bool m_IgnoreFromBuild;
#pragma warning disable IDE1006 // Naming Styles
        public bool ignoreFromBuild { get { return m_IgnoreFromBuild; } set { m_IgnoreFromBuild = value; } }
#pragma warning restore IDE1006 // Naming Styles

        // List of agent types the modifier is applied for.
        // Special values: empty == None, m_AffectedAgents[0] =-1 == All.
        [SerializeField]
        List<int> m_AffectedAgents = new List<int>(new int[] { -1 });    // Default value is All

        static readonly List<NavMeshModifier> s_NavMeshModifiers = new List<NavMeshModifier>();

#pragma warning disable IDE1006 // Naming Styles
        public static List<NavMeshModifier> activeModifiers
#pragma warning restore IDE1006 // Naming Styles
        {
            get { return s_NavMeshModifiers; }
        }

        void OnEnable()
        {
            if (!s_NavMeshModifiers.Contains(this))
                s_NavMeshModifiers.Add(this);
        }

        void OnDisable()
        {
            s_NavMeshModifiers.Remove(this);
        }

        public bool AffectsAgentType(int agentTypeID)
        {
            if (m_AffectedAgents.Count == 0)
                return false;
            if (m_AffectedAgents[0] == -1)
                return true;
            return m_AffectedAgents.IndexOf(agentTypeID) != -1;
        }
    }
}
