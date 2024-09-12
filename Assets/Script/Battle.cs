using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
public class Battle : MonoBehaviour
{
    public GameObject[] subjectPrefabs;// 워드 프리팹 배열
    public GameObject[] objectPrefabs;
    public GameObject[] verbPrefabs;
    public Transform objectParent;   // Object 부모 오브젝트
    public Transform verbParent;     // Verb 부모 오브젝트
    public Transform subjectParent;  // Subject 부모 오브젝트
    public float startX = -4f;       // 배치 시작 X 위치
    public float yPosition = -4f;    // 배치할 Y 위치
    public float spacing = 1f;       // 프리팹 간의 간격
    public int wordCount = 5;
    public int current = 0;

    public Button subjectButton;         // 주어 버튼
    public Button verbButton;            // 동사 버튼
    public Button objectButton;          // 목적어 버튼
    public GameObject selectionPanel;

    public Button Complete;

    
    // Start is called before the first frame update
    void Start()
    {
        subjectButton.onClick.AddListener(() => OnButtonClicked("Subject"));
        verbButton.onClick.AddListener(() => OnButtonClicked("Verb"));
        objectButton.onClick.AddListener(() => OnButtonClicked("Object"));
        Complete.onClick.AddListener(NextTurn);



        selectionPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (current > 4)
        {
            selectionPanel.SetActive(false);
            current = 0;
        }


    }

    void OnButtonClicked(string wordType)
    {
        // 랜덤으로 프리팹 선택


        if (wordType == "Subject")
        {
            GameObject randomPrefab = subjectPrefabs[Random.Range(0, subjectPrefabs.Length)];
            GameObject wordInstance = Instantiate(randomPrefab);
            wordInstance.transform.SetParent(subjectParent);
            float xPosition = startX + current * spacing;
            wordInstance.transform.position = new Vector3(xPosition, yPosition, -2f);
        }
        else if (wordType == "Verb")
        {
            GameObject randomPrefab = verbPrefabs[Random.Range(0, verbPrefabs.Length)];
            GameObject wordInstance = Instantiate(randomPrefab);
            wordInstance.transform.SetParent(verbParent);
            float xPosition = startX + current * spacing;
            wordInstance.transform.position = new Vector3(xPosition, yPosition, -2f);
        }
        else if (wordType == "Object")
        {
            GameObject randomPrefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
            GameObject wordInstance = Instantiate(randomPrefab);
            wordInstance.transform.SetParent(objectParent);
            float xPosition = startX + current * spacing;
            wordInstance.transform.position = new Vector3(xPosition, yPosition, -2f);
        }

        current++;

    }

    void NextTurn()
    {
        List<GameObject> Longest = FindLongest();
        Debug.Log(Longest.Count);
        if (Longest.Count == 3)
        {
            Debug.Log("세개");
            if (Longest[0].name.Contains("Subject") && Longest[1].name.Contains("Object") && Longest[2].name.Contains("Verb"))
            {
                if (Longest[2].name.Contains("Attack"))
                {
                    Attack(Longest[0], Longest[1]);
                }

                else if (Longest[2].name.Contains("Guard"))
                {
                    Guard(Longest[0], Longest[1]);
                }

                else if (Longest[2].name.Contains("Recover"))
                {
                    Recover(Longest[0], Longest[1]);
                }
            }
        }

        GameObject.FindGameObjectsWithTag("Word").ToList().ForEach(Destroy);

        selectionPanel.SetActive(true);
    }
    void Attack(GameObject word1, GameObject word2)
    {
        Status subject = GetStatusComponent(word1);
        Status target = GetStatusComponent(word2);

        target.currentHP = target.currentHP + target.currentGuard - subject.attackPower;
        Debug.Log("공격");
    }

    void Guard(GameObject word1, GameObject word2)
    {
        Status subject = GetStatusComponent(word1);
        Status target = GetStatusComponent(word2);

        target.currentGuard+=subject.guardPower;
        Debug.Log("가드");
    }

    void Recover(GameObject word1, GameObject word2)
    {
        Status subject = GetStatusComponent(word1);
        Status target = GetStatusComponent(word2);

        target.currentHP += subject.recoverPower;
        Debug.Log("회복");
    }

    Status GetStatusComponent(GameObject obj)
    {
        if (obj.name.Contains("Player"))
        {
            return GameObject.Find("Player").GetComponent<Status>();
        }
        else if (obj.name.Contains("Enemy"))
        {
            return GameObject.Find("Enemy").GetComponent<Status>();
        }
        return null;
    }

    List<GameObject> FindLongest()
    {
        // 씬에 있는 모든 'Word' 태그 오브젝트를 찾아 리스트로 만듦
        GameObject[] wordObjects = GameObject.FindGameObjectsWithTag("Word");
        List<GameObject> longestSequence = new List<GameObject>();

        // 각 오브젝트에 대해 가장 긴 연속된 배열을 찾음
        foreach (GameObject word in wordObjects)
        {
            if (word.transform.position.z != 0)
                continue;

            List<GameObject> currentSequence = new List<GameObject> { word };
            Vector3 currentPosition = word.transform.position;

            // 다음 오브젝트를 찾음
            while (true)
            {
                GameObject nextObject = FindNextObject(currentPosition, wordObjects);
                if (nextObject != null)
                {
                    currentSequence.Add(nextObject);
                    currentPosition = nextObject.transform.position;
                }
                else
                {
                    break;
                }
            }

            // 가장 긴 배열을 업데이트
            if (currentSequence.Count > longestSequence.Count)
            {
                longestSequence = new List<GameObject>(currentSequence);
            }
        }

        return longestSequence;
    }
    GameObject FindNextObject(Vector3 currentPosition, GameObject[] wordObjects)
    {
        GameObject nextObject = null;
        float closestDistance = float.MaxValue;
        float searchRange = 3f; // 두 오브젝트 간의 최대 거리 (적절히 조정 가능)

        foreach (GameObject word in wordObjects)
        {
            if (word.transform.position.z != 0)
                continue;

            if (word.transform.position == currentPosition) continue;

            float distance = word.transform.position.x - currentPosition.x;
            if (distance > 0 && distance <= searchRange)
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nextObject = word;
                }
            }
        }

        return nextObject;
    }

}