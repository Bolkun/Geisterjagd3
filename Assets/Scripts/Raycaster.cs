using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ARLocation;

public class Raycaster : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera;
    public GameObject ghostContainer;

    private bool start = true;
    private bool playing = false;
    private bool end = false;
    public LayerMask ignoreLayer;
    public AudioSource startSound;
    public AudioSource endSound;

    [SerializeField] 
    public Animator animator;
    private LookAtPlayer lookAtPlayerScript;
    private MoveAlongPath moveAlongPathScript;


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
        if (start == true) {
            this.animator.Play("castSpellB");
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 200.0f, ~ignoreLayer)) {
                    if (hit.transform != null && GetName(hit.transform.gameObject) == "GhostContainer") {
                        start = false;
                        startSound.Play();
                        this.animator.Play("idle");
                        lookAtPlayerScript.on = false;
                        moveAlongPathScript.Play();
                        playing = true;
                    }
                }
            }
        }

        if (playing == true && moveAlongPathScript.state.Playing == false) {
            playing = false;
            end = true;
        }

        if (end == true) {
            lookAtPlayerScript.on = true;
            this.animator.Play("castSpellA");
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 200.0f, ~ignoreLayer)) {
                    if (hit.transform != null && GetName(hit.transform.gameObject) == "GhostContainer") {
                        end = false;
                        endSound.Play();
                        this.animator.Play("death");
                        StartCoroutine( HandleEndScene() );
                    }
                }
            }
        } 
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
