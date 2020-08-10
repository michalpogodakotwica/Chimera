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
        private Material _walkableMaterial;
        [SerializeField] 
        private Material _defaultMaterial;
        [SerializeField] 
        private Material _pathMaterial;
        [SerializeField] 
        private Material _possibleTargetMaterial;
        [SerializeField] 
        private Material _controlledCharacterMaterial;
        [SerializeField] 
        private Material _targetMaterial;

        [SerializeField] 
        private Renderer _fieldRenderer;
        
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