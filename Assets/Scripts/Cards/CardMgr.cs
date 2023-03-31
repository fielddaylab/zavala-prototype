using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Sim;

namespace Zavala.Cards
{
    public class CardMgr : MonoBehaviour
    {
        public static CardMgr Instance;

        #region Inspector

        public GameObject SlotCardPrefab;

        #endregion // Inspector

        #region Card Definitions

        [SerializeField] private TextAsset m_cardDefs;

        private Dictionary<string, CardData> m_allCards;
        private List<string> m_unlockedCards;

        private static string HEADER_TAG = "@header";
        private static string SEVERITY_TAG = "@severity";
        private static string SIMID_TAG = "@simid";

        private static string END_DELIM = "\n";

        #endregion // Card Definitions

        #region Card Mappings

        private Dictionary<SimLeverID, List<string>> m_cardMap; // maps slot type to list of relevant cards

        #endregion // Card Mappings


        public void Init() {
            Instance = this;

            m_allCards = new Dictionary<string, CardData>();
            m_unlockedCards = new List<string>();

            InitCardMap();

            PopulateCards();
        }

        public List<CardData> GetOptions(SimLeverID slotType) {
            List<CardData> allOptions = new List<CardData>();

            List<string> cardIDs = m_cardMap[slotType];

            foreach(string cardID in cardIDs) {
                if (m_unlockedCards.Contains(cardID)) {
                    allOptions.Add(m_allCards[cardID]);
                }
            }

            return allOptions;
        }

        #region Helpers

        private void InitCardMap() {
            m_cardMap = new Dictionary<SimLeverID, List<string>>();

            foreach(SimLeverID lever in Enum.GetValues(typeof(SimLeverID))) {
                m_cardMap.Add(lever, new List<string>());
            }
        }

        private void PopulateCards() {
            List<string> cardStrings = TextIO.TextAssetToList(m_cardDefs, "::");

            foreach (string str in cardStrings) {
                try {
                    CardData newCard = ConvertDefToCard(str);

                    m_allCards.Add(newCard.CardID, newCard);
                    m_unlockedCards.Add(newCard.CardID); // temp hack

                    // add card to list of cards that should appear for the given sim id (queried when slot is selected)
                    List<string> relevant = m_cardMap[newCard.SimID];
                    relevant.Add(newCard.CardID);
                    m_cardMap[newCard.SimID] = relevant;

                    Debug.Log("[CardMgr] added " + newCard.CardID + " to the options for lever " + newCard.SimID.ToString());
                }
                catch (Exception e) {
                    Debug.Log("[CardMgr] Parsing error! " + e.Message);
                }
            }
        }

        private CardData ConvertDefToCard(string cardDef) {
            Debug.Log("[CardMgr] converting card: " + cardDef);
            string cardID = "";
            string header = "";
            Severity severity = Severity.None;
            SimLeverID simId = SimLeverID.RunoffPenalty;

            // Parse into data

            // First line must be card id
            cardID = cardDef.Substring(0, cardDef.IndexOf(END_DELIM));
            Debug.Log("[CardMgr] parsed card id : " + cardID);

            // Header comes after @Header
            int headerIndex = cardDef.ToLower().IndexOf(HEADER_TAG);
            if (headerIndex != -1) {
                string afterHeader = cardDef.Substring(headerIndex);
                int offset = HEADER_TAG.Length;
                header = cardDef.Substring(headerIndex + offset, afterHeader.IndexOf(END_DELIM) - offset).Trim();
            }
            else {
                // syntax error
                Debug.Log("[CardMgr] header syntax error!");
                
                throw new Exception("Header");
            }

            // Severity comes after @Severity
            int severityIndex = cardDef.ToLower().IndexOf(SEVERITY_TAG);
            if (severityIndex != -1) {
                string afterSeverity = cardDef.Substring(severityIndex);
                int offset = SEVERITY_TAG.Length;
                string severityStr = cardDef.Substring(severityIndex + offset, afterSeverity.IndexOf(END_DELIM) - offset).Trim();
                severity = (Severity)Enum.Parse(typeof(Severity), severityStr, true);
            }
            else {
                // syntax error
                Debug.Log("[CardMgr] severity syntax error!");

                throw new Exception("Severity");
            }

            // SimLeverID comes after @SimId
            int simIDIndex = cardDef.ToLower().IndexOf(SIMID_TAG);

            if (simIDIndex != -1) {
                string afterSimID = cardDef.Substring(simIDIndex);
                int offset = SIMID_TAG.Length;
                string simIDStr = cardDef.Substring(simIDIndex + offset, afterSimID.IndexOf(END_DELIM) - offset).Trim();

                simId = (SimLeverID)Enum.Parse(typeof(SimLeverID), simIDStr, true);
            }
            else {
                // syntax error
                Debug.Log("[CardMgr] sim lever syntax error!");

                throw new Exception("Sim Lever");
            }

            return new CardData(cardID, header, severity, simId);
        }

        #endregion // Helpers
    }
}
