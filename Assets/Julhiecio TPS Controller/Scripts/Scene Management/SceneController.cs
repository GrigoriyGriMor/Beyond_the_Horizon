using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("JU TPS/Scene Management/Scene Controller")]
public class SceneController : MonoBehaviour
{
    ThirdPersonController pl;
    public bool ResetLevelWhenPlayerDie;
    public float SecondsToReset = 4;
    public bool ExitGameWhenPressEsc;
    public bool ResetLevelWhenPressP;
    void Start()
    {
        pl = FindObjectOfType<ThirdPersonController>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && ExitGameWhenPressEsc == true)
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.P) && ResetLevelWhenPressP == true)
        {
            ResetLevel();
        }
        if(pl.IsDead == true && IsInvoking("ResetLevel") == false && ResetLevelWhenPlayerDie == true)
        {
            Invoke("ResetLevel", SecondsToReset);
        }
    }
    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
