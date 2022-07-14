using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Sim;

namespace Zavala
{
    public abstract class SimModeUI : MonoBehaviour
    {
        [Header("SimModeUI")]

        [SerializeField] protected string m_id;

        protected List<SimAction> m_completedActions;

        public string ID {
            get { return m_id; }
        }
        public List<SimAction> CompletedActions {
            get { return m_completedActions; }
        }

        protected void Awake() {
            m_completedActions = new List<SimAction>();
        }

        protected void OnEnable() {
            EventMgr.SimCanvasSubmitted?.AddListener(OnSimCanvasSubmitted);// maybe cause issues
            EventMgr.SimStageActions?.AddListener(OnSimStageActions);
            EventMgr.RegisterAction?.AddListener(OnRegisterAction);
            EventMgr.RemoveAction?.AddListener(OnRemoveAction);
        }

        protected void OnDisable() {
            EventMgr.SimCanvasSubmitted?.RemoveListener(OnSimCanvasSubmitted);// maybe cause issues
            EventMgr.SimStageActions?.RemoveListener(OnSimStageActions);
            EventMgr.RegisterAction?.RemoveListener(OnRegisterAction);
            EventMgr.RemoveAction?.RemoveListener(OnRemoveAction);
        }

        private void OnSimStageActions() {
            if (!this.gameObject.activeSelf) { return; }

            EventMgr.SimPostActions?.Invoke(m_completedActions);
        }

        protected void OnRegisterAction(SimAction action) {
            if (!this.gameObject.activeSelf) { return; }

            m_completedActions.Add(action);
        }

        protected void OnRemoveAction(SimAction action) {
            if (!this.gameObject.activeSelf) { return; }

            if (m_completedActions.Contains(action)) {
                m_completedActions.Remove(action);
            }
            else {
                Debug.Log("Warning! Tried to remove a sim action that had not been registered.");
            }
        }

        public abstract void Open();

        protected abstract void OnSimCanvasSubmitted();

    }
}