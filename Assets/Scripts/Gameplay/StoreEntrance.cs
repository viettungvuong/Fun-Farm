using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreEntrance : MonoBehaviour
{
    public StoreData store;

    public void OpenStore(){
        store.storePanel.SetActive(true);
    }

    public void CloseStore(){
        store.storePanel.SetActive(false);
    }
}
