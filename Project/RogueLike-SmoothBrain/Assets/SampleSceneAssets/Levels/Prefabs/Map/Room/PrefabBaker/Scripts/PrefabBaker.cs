using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrefabLightMapBaker
{
    [ExecuteInEditMode]
    public class PrefabBaker : MonoBehaviour
    {
        [SerializeField] public LightInfo[] lights;
        [SerializeField] public Renderer[] renderers;
        [SerializeField] public int[] renderersLightmapIndex;
        [SerializeField] public Vector4[] renderersLightmapOffsetScale;
        [SerializeField] public Texture2D[] texturesColor;
        [SerializeField] public Texture2D[] texturesDir;
        [SerializeField] public Texture2D[] texturesShadow;

        public Texture2D[][] AllTextures() => new Texture2D[][] {
            texturesColor, texturesDir, texturesShadow
        };

        public bool HasBakeData => (renderers?.Length ?? 0) > 0 && (texturesColor?.Length ?? 0) > 0;

        private bool bakeCalled = false;
        public bool BakeApplied
        {
            get
            {
                bool hasColors = Utils.SceneHasAllLightmaps(texturesColor);
                bool hasDirs = Utils.SceneHasAllLightmaps(texturesDir);
                bool hasShadows = Utils.SceneHasAllLightmaps(texturesShadow);

                return hasColors && hasDirs && hasShadows && bakeCalled;
            }
        }

        void Start()
        {
            BakeApply();

            // Warnning : this will mess up the renderer lightmaps reference
            // // StaticBatchingUtility.Combine( gameObject );
        }
        public bool BakeJustApplied { private set; get; } = false;

        public void BakeApply()
        {
            if (!HasBakeData)
            {
                BakeJustApplied = false;
                Debug.LogWarning("PrefabBaker doesn't have bake data", this);
                return;
            }

            if (!BakeApplied)
            {
                BakeJustApplied = Utils.Apply(this);

                if (BakeJustApplied)
                {
                    //Debug.Log("[PrefabBaker] Addeded prefab lightmap data to current scene");
                }
            }

            bakeCalled = true;
        }

        void OnEnable()
        {
            if (!Application.isPlaying)
            {
                BakeApply();
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            BakeApply();
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public static System.Action onValidate;

        private void OnValidate()
        {
            onValidate?.Invoke();
        }
    }
}