using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

namespace UI {
    public class UIManager : MonoBehaviour {
        public Text m_CurrentUnit;
        private GameObject ActiveTarget;
        private void Start() {
            m_CurrentUnit.text = "ActiveTarget.name";
        }

        protected void Update() {
        }
        public void SetActiveTarget(GameObject Target) {
            ActiveTarget = Target;
            m_CurrentUnit.text = ActiveTarget.name;
        }
        
        public void SetMap(bool map) {
            if (map){
                m_CurrentUnit.enabled = false;
            } else {
                m_CurrentUnit.enabled = true;
            }    
        }
    }
}