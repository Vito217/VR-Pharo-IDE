using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CodeCube : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 1000f;
    public float scalingSpeed = 5f;

    private Transform baseParent;
    protected bool isDragged = false;
   
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<XRSimpleInteractable>().interactionManager = 
            GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>();

        InnerStart();
    }

    void Update()
    {
        if (isDragged)
        {
            VRIDEInputHandler inputs = transform.parent.root.GetComponent<VRIDEInputHandler>();
            if (inputs.LeftPrimaryButtonDown || inputs.RightPrimaryButtonDown)
                Destroy(gameObject);                
        }
    }

    public virtual void InnerStart()
    {
        
    }

    public void RotateLocally(float degrees)
    {
        StartCoroutine(RotateLocallyBy(degrees));
    }

    public void MoveLocallyTo(Vector3 target)
    {
        StartCoroutine(MoveLocallyToTarget(target));
    }

    public void ScaleTo(Vector3 target)
    {
        StartCoroutine(ScaleBy(target));
    }

    private IEnumerator MoveLocallyToTarget(Vector3 target)
    {
        while(transform.localPosition != target)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition, target, Time.deltaTime * movementSpeed
            );

            yield return null;
        }
    }

    private IEnumerator RotateLocallyBy(float degrees)
    {
        float degreeCounter = 0f;
        while(Mathf.Abs(degreeCounter) <= Mathf.Abs(degrees))
        {
            float degreesRotated = rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, degreesRotated, 0f);
            degreeCounter += degreesRotated;
            yield return null;
        }
    }

    private IEnumerator ScaleBy(Vector3 target)
    {
        while (transform.localScale != target)
        {
            transform.localScale = Vector3.MoveTowards(
                transform.localScale, target, Time.deltaTime * scalingSpeed
            );

            yield return null;
        }
    }

    public void OnSelectEnter(SelectEnterEventArgs eventArgs)
    {
        isDragged = true;
        baseParent = transform.parent;
        transform.SetParent(eventArgs.interactor.transform);

        Transform text = transform.Find("CodeCubeText(Clone)");
        if (text) Destroy(text.gameObject);
    }

    public void OnSelectExit()
    {
        isDragged = false;
        transform.SetParent(baseParent);
    }
}
