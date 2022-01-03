using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private InputField nameField;
	public Button back;
	private void Start()
	{		
		if(PlayerPrefs.HasKey("Player_Name"))
		nameField.text=PlayerPrefs.GetString("Player_Name");
	}
	public void  OnEndEditName()
	{
		PlayerPrefs.SetString("Player_Name",nameField.text);
    }

    public void LoadTo(int level)
    {
        SceneManager.LoadScene(level);
    }
    public void Back()
    {
		back.interactable = true;
    }
 public void OnClickPlay()
 {
 SceneManager.LoadScene(1);
 }
 public void OnClickExit()
 {
 Application.Quit();
 }
	
}
