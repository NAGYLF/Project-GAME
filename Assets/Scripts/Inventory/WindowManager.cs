using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MainData.SupportScripts;
using ItemHandler;
using UnityEditor.PackageManager.UI;

namespace Assets.Scripts.Inventory
{
    public class WindowManager : MonoBehaviour
    {
        const string modificationWindow = "GameElements/ModificationWindow";
        public List<GameObject> windows;
        private void Start()
        {
            windows = new List<GameObject>();
        }
        public void CreatePopUpWindow(AdvancedItem item)
        {
            GameObject window = CreatePrefab("GameElements/ItemWindow");
            window.transform.SetParent(gameObject.transform);//a window manager a payler inventoryn van
            window.GetComponent<ItemWindow>().positioning(item);
            item.SelfGameobject.GetComponent<ItemObject>().Window = window;
            windows.Add(window);
            windows.OrderBy(item => item.transform.GetSiblingIndex());
        }
        public void CreateModificationPanel(AdvancedItem item)
        {
            GameObject window = CreatePrefab(modificationWindow);
            window.transform.SetParent(gameObject.transform);
            window.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            windows.Add(window);
            window.GetComponent<ModificationWindow>().Openwindow(item);
            windows.OrderBy(item=>item.transform.GetSiblingIndex());
        }
        public void DestroyWindow(GameObject ToDestroyWindow)
        {
            windows.Remove(ToDestroyWindow);
            Destroy(ToDestroyWindow);
        }
        public void SetWindowToTheTop(GameObject ToTopWindow)
        {
            ToTopWindow.transform.SetSiblingIndex(windows.Last().transform.GetSiblingIndex()+1);
        }
        public void ClearWindowManager()
        {
            foreach (GameObject window in windows)
            {
                Destroy(window);
            }
            windows.Clear();
        }
    }
}
