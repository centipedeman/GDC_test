using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MoveWord : MonoBehaviour
{
    private bool isDragging = false; // �巡�� ������ Ȯ��
    private Vector3 offset; // ���콺�� ������Ʈ ������ ������
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;

    private float snapDistance = 3f; // ������ �Ÿ��� �ִ밪
    private List<GameObject> connectedObjects = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSortingOrder = spriteRenderer.sortingOrder;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            Vector3 newPosition = mousePosition + offset;

            Vector3 deltaPosition = newPosition - transform.position;

            transform.position = newPosition;

            foreach (GameObject connectedObject in connectedObjects)
            {
                connectedObject.transform.position += deltaPosition;
            }
        }


    }

    void OnMouseDown()
    {
        transform.position = new Vector3 (transform.position.x, transform.position.y,0f); 
        isDragging = true;
        Vector3 mousePosition = GetMouseWorldPosition();
        offset = transform.position - mousePosition;

        spriteRenderer.sortingOrder = 100;

        connectedObjects = FindConnectedObjects();
    }

    void OnMouseUp()
    {
        isDragging = false;
        GameObject closestObject = FindClosestObject();

        UnityEngine.Debug.Log('1');

        if (closestObject != null)
        {
            SpriteRenderer closestSprite = closestObject.GetComponent<SpriteRenderer>();
            SpriteRenderer currentSprite = GetComponent<SpriteRenderer>();

            // ���� ����� ������Ʈ�� �����ʿ� ��ġ��Ŵ
            if (closestSprite != null && currentSprite != null)
            {
                UnityEngine.Debug.Log('2');
                float offsetRight = closestSprite.bounds.size.x / 2 + currentSprite.bounds.size.x / 2;

                if (transform.position.x > closestObject.transform.position.x )
                {
                    UnityEngine.Debug.Log('6');
                    GameObject rightObject = FindRightObject(closestObject);

                    // �����ʿ� ���̱�
                    
                    

                    if (rightObject != null)
                    {
                        UnityEngine.Debug.Log('7');
                        SpriteRenderer rightSprite = rightObject.GetComponent<SpriteRenderer>();
                        

                        if (rightSprite != null)
                        {
                            transform.position = new Vector3(
                            closestObject.transform.position.x + offsetRight,
                            closestObject.transform.position.y,
                            transform.position.z
                            );

                            rightObject.transform.position = new Vector3(
                                closestObject.transform.position.x + offsetRight*2,
                                closestObject.transform.position.y,
                                rightObject.transform.position.z
                            );
                        }

                        

                    }
                    
                    else
                    {
                        transform.position = new Vector3(
                        closestObject.transform.position.x + offsetRight,
                        closestObject.transform.position.y,
                        transform.position.z
                        );

                        
                    }


                }
                else
                {
                    UnityEngine.Debug.Log('3');

                    GameObject leftObject = FindLeftObject(closestObject);
                    // ���ʿ� ���̱�


                    if (leftObject != null)
                    {
                        UnityEngine.Debug.Log('4');
                        SpriteRenderer leftSprite = leftObject.GetComponent<SpriteRenderer>();
                        if (leftSprite != null)
                        {
                            transform.position = new Vector3(
                                leftObject.transform.position.x + leftSprite.bounds.size.x / 2 + currentSprite.bounds.size.x / 2,
                                leftObject.transform.position.y,
                                transform.position.z
                            );

                            closestObject.transform.position = new Vector3(
                                transform.position.x + currentSprite.bounds.size.x / 2 + leftSprite.bounds.size.x / 2,
                                transform.position.y,
                                closestObject.transform.position.z
                            );
                        }

                        
                    }

                    else
                    {
                        transform.position = new Vector3(
                        closestObject.transform.position.x - offsetRight,
                        closestObject.transform.position.y,
                        transform.position.z
                        );

                        if (connectedObjects.Count > 0)
                        {
                            GameObject rightMostObject = connectedObjects[0];

                            foreach (GameObject obj in connectedObjects)
                            {
                                if (obj.transform.position.x > rightMostObject.transform.position.x)
                                {
                                    rightMostObject = obj;
                                }
                            }

                            GameObject newClosestObject = FindClosestObjectToRight(rightMostObject);

                            if (newClosestObject != null)
                            {
                                SpriteRenderer rightMostSprite = rightMostObject.GetComponent<SpriteRenderer>();
                                SpriteRenderer newClosestSprite = newClosestObject.GetComponent<SpriteRenderer>();

                                if (rightMostSprite != null && newClosestSprite != null)
                                {


                                    // ���ο� ���� ����� ������Ʈ�� ���� ������ ������Ʈ�� �����ʿ� ����
                                    newClosestObject.transform.position = new Vector3(
                                        rightMostObject.transform.position.x + rightMostSprite.bounds.size.x / 2 + newClosestSprite.bounds.size.x / 2,
                                        rightMostObject.transform.position.y,
                                        rightMostObject.transform.position.z
                                    );
                                }
                            }
                        }
                        

                    }
                }

            }
        }
        

        spriteRenderer.sortingOrder = originalSortingOrder;
    }


    private Vector3 GetMouseWorldPosition()
    {
        // ���콺 �������� ���� ���������� ��ȯ
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z; // ������Ʈ�� z ���� ����
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }

    private GameObject FindClosestObject()
    {
        GameObject[] words = GameObject.FindGameObjectsWithTag("Word"); // ��� �ܾ� ������Ʈ ã�� (�±� ���)
        GameObject closestObject = null;
        float closestDistance = snapDistance;

        foreach (GameObject word in words)
        {
            if (word == gameObject) continue; // �ڱ� �ڽ��� ����

            float distance = Vector3.Distance(transform.position, word.transform.position);
            

            if (distance < closestDistance && word.transform.position.z == transform.position.z)
            {
                closestDistance = distance;
                closestObject = word;
            }
        }
        
        return closestObject;
    }

    private GameObject FindRightObject(GameObject referenceObject)
    {
        GameObject[] words = GameObject.FindGameObjectsWithTag("Word"); // ��� �ܾ� ������Ʈ ã�� (�±� ���)
        GameObject rightObject = null;
        float closestDistance = snapDistance;



        foreach (GameObject word in words)
        {
            if (word == referenceObject || word == gameObject) continue; // �ڱ� �ڽŰ� ���� ������Ʈ�� ����

            float distance = word.transform.position.x - gameObject.transform.position.x;

            if (distance > 0 && distance < snapDistance && word.transform.position.z == transform.position.z)
            {
                
                rightObject = word;
            }
        }
        UnityEngine.Debug.Log(referenceObject);
        return rightObject;
    }

    private GameObject FindLeftObject(GameObject referenceObject)
    {
        GameObject[] words = GameObject.FindGameObjectsWithTag("Word"); // ��� �ܾ� ������Ʈ ã�� (�±� ���)
        GameObject leftObject = null;
        float closestDistance = snapDistance;

        foreach (GameObject word in words)
        {
            if (word == referenceObject || word == gameObject) continue; // �ڱ� �ڽŰ� ���� ������Ʈ�� ����

            float distance = gameObject.transform.position.x - word.transform.position.x;

            if (distance > 0 && distance < closestDistance && word.transform.position.z == transform.position.z)
            {
                closestDistance = distance;
                leftObject = word;
            }
        }

        return leftObject;
    }

    private List<GameObject> FindConnectedObjects()
    {
        List<GameObject> connectedObjects = new List<GameObject>();
        Queue<GameObject> queue = new Queue<GameObject>();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        queue.Enqueue(gameObject); // ���� ������Ʈ�� �ڽ�
        visited.Add(gameObject);

        while (queue.Count > 0)
        {
            GameObject current = queue.Dequeue();
            connectedObjects.Add(current);

            GameObject[] words = GameObject.FindGameObjectsWithTag("Word");

            foreach (GameObject word in words)
                {
                if (visited.Contains(word)) continue; // �̹� �湮�� ������Ʈ�� �ǳʶ�

                float distance = Vector3.Distance(current.transform.position, word.transform.position);

                // �����ʿ� �ְ�, ���� �Ÿ� �ȿ� �ִ� ������Ʈ�� �߰�
                if (distance < snapDistance && word.transform.position.x > current.transform.position.x && word.transform.position.z == transform.position.z)
                {
                    queue.Enqueue(word);
                    visited.Add(word);
                }
            }
        }

        connectedObjects.Remove(gameObject); // �ڱ� �ڽ��� ����Ʈ���� ����
        return connectedObjects;
    }

    private GameObject FindClosestObjectToRight(GameObject referenceObject)
    {
        GameObject[] words = GameObject.FindGameObjectsWithTag("Word"); // ��� �ܾ� ������Ʈ ã�� (�±� ���)
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject word in words)
        {
            if (word == referenceObject || word == gameObject) continue; // �ڱ� �ڽŰ� ���� ������Ʈ�� ����

            float distance = word.transform.position.x - referenceObject.transform.position.x;

            if (distance > 0 && distance < closestDistance && word.transform.position.z == transform.position.z)
            if (distance > 0 && distance < closestDistance && word.transform.position.z == transform.position.z)
            {
                closestDistance = distance;
                closestObject = word;
            }
        }

        return closestObject;
    }
}
