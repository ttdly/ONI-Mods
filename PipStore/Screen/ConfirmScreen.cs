using UnityEngine.UI;
using PipStore.Screen.Basic;

namespace PipStore.Screen {
    public class ConfirmScreen : KScreen {
        public LocText confirmMsg;
        public FNumberInputField fNumberInputField;
        private readonly string rawHint = string.Copy(PipStoreString.Screen.ConfirmMsg);
        
        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            gameObject.transform.Find("Content/Title/Label")
                .gameObject.GetComponent<LocText>().SetText(PipStoreString.Screen.ConfirmTitle);
            gameObject.transform.Find("Content/Ok/Label")
                .gameObject.GetComponent<LocText>().SetText(PipStoreString.Screen.Confirm);
            gameObject.transform.Find("Content/NumControl/Sub/Label").gameObject.GetComponent<LocText>().SetText("-");
            gameObject.transform.Find("Content/NumControl/Add/Label").gameObject.GetComponent<LocText>().SetText("+");
            gameObject.transform.Find("Content/Title/Close").gameObject.GetComponent<Button>().onClick.AddListener(Close);
            gameObject.transform.Find("Content/Ok")
                .gameObject.GetComponent<Button>().onClick.AddListener(Confirm);
            gameObject.transform.Find("Content/NumControl/Sub").gameObject.GetComponent<Button>()
                .onClick.AddListener(Sub);
            gameObject.transform.Find("Content/NumControl/Add").gameObject.GetComponent<Button>()
                .onClick.AddListener(Add);
        }
        protected override void OnSpawn() {
            base.OnSpawn();
            fNumberInputField.inputField.onEndEdit.AddListener(RefreshMsg);
        }
        private void RefreshMsg(string text) {
            var hint = rawHint
                .Replace("{coin}",
                    (PipStoreScreen.Instance.currentGoods.goodsPrice * fNumberInputField.GetFloat).ToString("0.00"))
                .Replace("{count}", text + PipStoreScreen.Instance.currentGoods.GetSpawnableQuantityOnly())
                .Replace("{item}", PipStoreScreen.Instance.currentGoods.goodsProperName);
            if (confirmMsg == null) {
                confirmMsg = gameObject.transform.Find("Content/Hint").GetComponent<LocText>();
            }
            confirmMsg.SetText(hint);
        }
        public void Show() {
            fNumberInputField.SetTextFromData("1");
            RefreshMsg("1");
            gameObject.SetActive(true);
        }
        public void Close() {
            PipStoreScreen.Instance.CancelPurchase();
            gameObject.SetActive(false);
        }
        public void Confirm() {
            PipStoreScreen.Instance.DoPurchase((int)fNumberInputField.GetFloat);
            gameObject.SetActive(false);
        }
        public void Sub() {
            var num = fNumberInputField.GetFloat;
            var numStr = (num - 1).ToString("0");
            fNumberInputField.SetTextFromData(numStr);
            RefreshMsg(numStr);
        }
        public void Add() {
            var num = fNumberInputField.GetFloat;
            var numStr = (num + 1).ToString("0");
            fNumberInputField.SetTextFromData(numStr);
            RefreshMsg(numStr);
        }
    }
}