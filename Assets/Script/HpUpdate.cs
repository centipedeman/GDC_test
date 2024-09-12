using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HpUpdate : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;  // Inspector에서 연결할 TextMeshPro 컴포넌트
    public Status status;  // HP를 관리하는 Status 스크립트
   

    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        if (textMeshPro != null && status != null)
        {
            textMeshPro.text = "HP: " + status.currentHP.ToString();
        }
    }
}
