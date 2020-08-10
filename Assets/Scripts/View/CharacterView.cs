using System;
using TMPro;
using UnityEngine;

namespace View
{
    public class CharacterView : MonoBehaviour, ICharacterView, IDisposable
    {
        [SerializeField]
        private TextMeshProUGUI _healthText = default;

        [SerializeField] 
        private Material[] _teamsMaterials = default;

        [SerializeField] 
        private Renderer _characterRenderer = default;
        
        public void SetTeam(int team)
        {
            _characterRenderer.material = _teamsMaterials[team];
        }
        
        public void SetCharacterHealth(int health)
        {
            _healthText.text = health.ToString();
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}