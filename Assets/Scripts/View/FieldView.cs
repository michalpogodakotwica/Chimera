using System;
using System.Collections.Generic;
using Input;
using UnityEngine;

namespace View
{
    [RequireComponent(typeof(RayCastDispatcher))]
    public class FieldView : MonoBehaviour, IFieldView, IDisposable
    {
        [SerializeField] 
        private Material _walkableMaterial = default;
        [SerializeField] 
        private Material _defaultMaterial = default;
        [SerializeField] 
        private Material _pathMaterial = default;
        [SerializeField] 
        private Material _possibleTargetMaterial = default;
        [SerializeField] 
        private Material _controlledCharacterMaterial = default;
        [SerializeField] 
        private Material _targetMaterial = default;

        [SerializeField] 
        private Renderer _fieldRenderer = default;
        
        private Dictionary<FieldHighlightType, Material> _highlightToMaterial;
        
        public void Awake()
        {
            _highlightToMaterial = new Dictionary<FieldHighlightType, Material>
            {
                {FieldHighlightType.Default, _defaultMaterial},
                {FieldHighlightType.Walkable, _walkableMaterial},
                {FieldHighlightType.Path, _pathMaterial},
                {FieldHighlightType.PossibleTarget, _possibleTargetMaterial},
                {FieldHighlightType.ActiveCharacter, _controlledCharacterMaterial},
                {FieldHighlightType.Target, _targetMaterial},
            };
        }

        public void SetFieldWalkable(bool isWalkable)
        {
            gameObject.SetActive(isWalkable);
        }

        public void SetHighlight(FieldHighlightType fieldHighlightType)
        {
            _fieldRenderer.material = _highlightToMaterial[fieldHighlightType];
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}