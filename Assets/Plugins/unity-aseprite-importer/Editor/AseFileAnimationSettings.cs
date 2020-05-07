using UnityEngine;

namespace AsepriteImporter
{
    [System.Serializable]
    public class AseFileAnimationSettings
    {

        public AseFileAnimationSettings()
        {
        }

        public AseFileAnimationSettings(string name)
        {
            animationName = name;
        }

        [SerializeField] public string animationName;
        [SerializeField] public bool loopTime = true;
        [SerializeField] public string about;
        [SerializeField] public Vector2 pivot = Vector2.one * 0.5f;

        public override string ToString()
        {
            return animationName;
        }
    }
}