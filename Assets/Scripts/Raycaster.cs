using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ARLocation;
using TMPro;

public class Raycaster : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera;
    public GameObject ghostContainer;

    [SerializeField]
    private TextMeshProUGUI m_TextComponent;

    private bool start = true;
    private bool playing = false;
    private int ignoreOneTime = 0;
    private bool pause = false;
    private bool end = false;
    public LayerMask ignoreLayer;
    public AudioSource startSound;
    public AudioSource pauseSound;
    public AudioSource resumeSound;
    public AudioSource endSound;

    [SerializeField] 
    public Animator animator;
    private LookAtPlayer lookAtPlayerScript;
    private MoveAlongPath moveAlongPathScript;

    private Vector3[] Points;
    private Spline spline;
    private int locPointCount;
    private Location[] locPoints;
    private Location ghostCurrentLocation;

    /// <summary>
    /// Initialize 2 scripts
    /// </summary>
    void Start()
    {
        lookAtPlayerScript = ghostContainer.GetComponent<LookAtPlayer>();
        moveAlongPathScript = ghostContainer.GetComponent<MoveAlongPath>();
    }

    /// <summary>
    /// 3 Type of states that reaper object can take: start, playing, end
    /// </summary>
    void Update()
    {
        // Autoplay on!
        // m_TextComponent.text = "PointCount: " +moveAlongPathScript.state.PointCount + 
        //                         "\nPoints: " +moveAlongPathScript.state.Points;
        // foreach (Vector3 vector3 in moveAlongPathScript.state.Points)
        // {
        //     Debug.Log(vector3);
        //     m_TextComponent.text = "PointCount: " +moveAlongPathScript.state.Points.Length +"\nPoints: " +vector3
        //                             +"\n SplineLength: "+ moveAlongPathScript.state.Spline.Points;
        // }
        locPointCount = moveAlongPathScript.state.PointCount;
        locPoints = moveAlongPathScript.PathSettings.LocationPath.Locations;
        Debug.Log(locPointCount);
        Debug.Log(locPoints);

        if (locPointCount >= 2) {
            ghostCurrentLocation = moveAlongPathScript.locationProvider.CurrentLocation.ToLocation();
            m_TextComponent.text = "loc: " +ghostCurrentLocation;
            for (var i = 0; i < locPointCount; i++) {
                if (ghostCurrentLocation.Latitude == locPoints[i].Latitude &&
                    ghostCurrentLocation.Longitude == locPoints[i].Longitude) {
                    moveAlongPathScript.Pause();
                    pause = true;
                }
            }
        }

        Points = moveAlongPathScript.state.Points;
        m_TextComponent.text = "Points: " +Points[0] +" " + Points[1] +" " +Points[2] +" " +Points[3] + " Length: " +Points.Length;
        for (var i = 0; i < locPointCount; i++) {
            Debug.Log("i="+i + " " + Points[i]);
        }

        spline = moveAlongPathScript.state.Spline;
        if (spline != null)
            m_TextComponent.text = "Spline: " +spline.Points[0] +spline.Points[1]+spline.Points[2]+spline.Points[3] + " Length: " +spline.Length;

        if (pause == true) {
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);

                // Resume
                if (Physics.Raycast(ray, out hit, 200.0f, ~ignoreLayer)) {
                    if (hit.transform != null && GetName(hit.transform.gameObject) == "GhostContainer") {
                        resumeSound.Play();
                        this.animator.Play("idle");
                        moveAlongPathScript.Play();
                        pause = false;
                    }
                }
            }
        } 
        
       

        // Start
        // if (start == true) {
        //     this.animator.Play("castSpellB");
        //     if (Input.GetMouseButtonDown(0)) {
        //         RaycastHit hit;
        //         Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);

        //         if (Physics.Raycast(ray, out hit, 200.0f, ~ignoreLayer)) {
        //             if (hit.transform != null && GetName(hit.transform.gameObject) == "GhostContainer") {
        //                 start = false;
        //                 startSound.Play();
        //                 this.animator.Play("idle");
        //                 lookAtPlayerScript.on = false;
        //                 moveAlongPathScript.Play();
        //                 playing = true;
        //             }
        //         }
        //     }
        // }

        // // Play
        // if (playing == true) {
        //     if (moveAlongPathScript.endReached == false) {  
        //         if (pause == true) {
        //             if (Input.GetMouseButtonDown(0)) {
        //                 RaycastHit hit;
        //                 Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);

        //                 // Resume
        //                 if (Physics.Raycast(ray, out hit, 200.0f, ~ignoreLayer)) {
        //                     if (hit.transform != null && GetName(hit.transform.gameObject) == "GhostContainer") {
        //                         resumeSound.Play();
        //                         this.animator.Play("idle");
        //                         moveAlongPathScript.Play();
        //                         pause = false;
        //                     }
        //                 }
        //             }
        //         } else {
        //             if (Input.GetMouseButtonDown(0) && ignoreOneTime == 1) {
        //                 RaycastHit hit;
        //                 Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);
                        
        //                 // Pause
        //                 if (Physics.Raycast(ray, out hit, 200.0f, ~ignoreLayer)) {
        //                     if (hit.transform != null && GetName(hit.transform.gameObject) == "GhostContainer") {
        //                         pauseSound.Play();
        //                         this.animator.Play("idle");
        //                         moveAlongPathScript.Pause();
        //                         pause = true;
        //                     }
        //                 }
        //             }
        //         }
        //         ignoreOneTime = 1;
        //     } else {
        //         playing = false;
        //         end = true;
        //     }
        // }

        // // End
        // if (end == true) {
        //     lookAtPlayerScript.on = true;
        //     this.animator.Play("castSpellA");
        //     if (Input.GetMouseButtonDown(0)) {
        //         RaycastHit hit;
        //         Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);

        //         if (Physics.Raycast(ray, out hit, 200.0f, ~ignoreLayer)) {
        //             if (hit.transform != null && GetName(hit.transform.gameObject) == "GhostContainer") {
        //                 end = false;
        //                 endSound.Play();
        //                 this.animator.Play("death");
        //                 StartCoroutine( HandleEndScene() );
        //             }
        //         }
        //     }
        // } 
    }

    /// <summary>
    /// Wait till animation plays till the end, than destroy reaper object
    /// </summary>
    private IEnumerator HandleEndScene() {
        yield return new WaitForSeconds(3.5f);
        Destroy(ghostContainer);
    }

    /// <summary>
    /// Name of hited object
    /// </summary>
    private string GetName(GameObject go) {
        return go.name;
    }
}
