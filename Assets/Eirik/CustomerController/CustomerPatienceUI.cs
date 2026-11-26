using UnityEngine;
using UnityEngine.UI;

public class CustomerPatienceUI : MonoBehaviour
{
    [SerializeField] private CustomerController customer;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image patienceFill;
    [SerializeField] private Color lowPatienceColor = Color.red;
    [SerializeField] private Color highPatienceColor = Color.green;

    private void Update()
    {
        if (customer == null || canvas == null || patienceFill == null)
        {
            return;
        }

        canvas.enabled = customer.ShouldDecreasePatience();
       
        patienceFill.fillAmount = customer.PatienceNormalized;
        patienceFill.color = Color.Lerp(lowPatienceColor, highPatienceColor, customer.PatienceNormalized);
    }

    private void LateUpdate()
    {
        if (canvas == null || mainCamera == null)
        {
            return;
        }

        canvas.transform.rotation = mainCamera.transform.rotation; //canvas.LookAt(mainCamera);
    }
}
