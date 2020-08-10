using System;
using TMPro;
using UnityEngine;

namespace View
{
    public class CharacterView : MonoBehaviour, ICharacterView, IDisposable
    {
        [SerializeField]
        private TextMeshProUGUI _healthText;

        [SerializeField] 
        private Material[] _teamsMaterials;

        [SerializeField] 
        private Renderer _characterRenderer;
        
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