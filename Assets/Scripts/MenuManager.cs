using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    private void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    public void AB2MaxMatch(string value)
    {
        try{
            MatchmakingManager.Instance.AB2MaxMatch = int.Parse(value);
        }
        catch (System.Exception){
            MatchmakingManager.Instance.AB2MaxMatch = 0;
        }
    }

    public void AB3MaxMatch(string value)
    {
        try{
            MatchmakingManager.Instance.AB3MaxMatch = int.Parse(value);
        }
        catch (System.Exception){
            MatchmakingManager.Instance.AB3MaxMatch = 0;
        }
    }

    public void AB4MaxMatch(string value)
    {
        try{
            MatchmakingManager.Instance.AB4MaxMatch = int.Parse(value);
        }
        catch (System.Exception){
            MatchmakingManager.Instance.AB4MaxMatch = 0;
        }
    }

    public void AB5MaxMatch(string value)
    {
        try{
            MatchmakingManager.Instance.AB5MaxMatch = int.Parse(value);
        }
        catch (System.Exception){
            MatchmakingManager.Instance.AB5MaxMatch = 0;
        }
    }

    public void KH2MaxMatch(string value)
    {
        try{
            MatchmakingManager.Instance.KH2MaxMatch = int.Parse(value);
        }
        catch (System.Exception){
            MatchmakingManager.Instance.KH2MaxMatch = 0;
        }
    }

    public void KH3MaxMatch(string value)
    {
        try{
            MatchmakingManager.Instance.KH3MaxMatch = int.Parse(value);
        }
        catch (System.Exception){
            MatchmakingManager.Instance.KH3MaxMatch = 0;
        }
    }

    public void KH4MaxMatch(string value)
    {
        try{
            MatchmakingManager.Instance.KH4MaxMatch = int.Parse(value);
        }
        catch (System.Exception){
            MatchmakingManager.Instance.KH4MaxMatch = 0;
        }
    }

    public void KH5MaxMatch(string value)
    {
        try{
            MatchmakingManager.Instance.KH5MaxMatch = int.Parse(value);
        }
        catch (System.Exception){
            MatchmakingManager.Instance.KH5MaxMatch = 0;
        }
    }

    public void StartMatch()
    {
        if(MatchmakingManager.Instance.FindRandomMatch())
            SceneManager.LoadScene("Gameplay");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
