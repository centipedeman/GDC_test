using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HpUpdate : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;  // Inspector���� ������ TextMeshPro ������Ʈ
    public Status status;  // HP�� �����ϴ� Status ��ũ��Ʈ
   

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
