using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
public class Battle : MonoBehaviour
{
    public GameObject[] subjectPrefabs;// ���� ������ �迭
    public GameObject[] objectPrefabs;
    public GameObject[] verbPrefabs;
    public Transform objectParent;   // Object �θ� ������Ʈ
    public Transform verbParent;     // Verb �θ� ������Ʈ
    public Transform subjectParent;  // Subject �θ� ������Ʈ
    public float startX = -4f;       // ��ġ ���� X ��ġ
    public float yPosition = -4f;    // ��ġ�� Y ��ġ
    public float spacing = 1f;       // ������ ���� ����
    public int wordCount = 5;
    public int current = 0;

    public Button subjectButton;         // �־� ��ư
    public Button verbButton;            // ���� ��ư
    public Button objectButton;          // ������ ��ư
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
        // �������� ������ ����


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
            Debug.Log("����");
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
        Debug.Log("����");
    }

    void Guard(GameObject word1, GameObject word2)
    {
        Status subject = GetStatusComponent(word1);
        Status target = GetStatusComponent(word2);

        target.currentGuard+=subject.guardPower;
        Debug.Log("����");
    }

    void Recover(GameObject word1, GameObject word2)
    {
        Status subject = GetStatusComponent(word1);
        Status target = GetStatusComponent(word2);

        target.currentHP += subject.recoverPower;
        Debug.Log("ȸ��");
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
        // ���� �ִ� ��� 'Word' �±� ������Ʈ�� ã�� ����Ʈ�� ����
        GameObject[] wordObjects = GameObject.FindGameObjectsWithTag("Word");
        List<GameObject> longestSequence = new List<GameObject>();

        // �� ������Ʈ�� ���� ���� �� ���ӵ� �迭�� ã��
        foreach (GameObject word in wordObjects)
        {
            if (word.transform.position.z != 0)
                continue;

            List<GameObject> currentSequence = new List<GameObject> { word };
            Vector3 currentPosition = word.transform.position;

            // ���� ������Ʈ�� ã��
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

            // ���� �� �迭�� ������Ʈ
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
        float searchRange = 3f; // �� ������Ʈ ���� �ִ� �Ÿ� (������ ���� ����)

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