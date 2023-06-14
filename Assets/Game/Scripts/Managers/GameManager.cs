using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Status")]
    public int levelGame;
    private int levelFixed;
    public bool isComplete;
    public int swipeAmount;

    [Header("Level Controller")]
    public int paintAmount;
    public int maxPaintAmount;
    public Ball yellowBall;
    public Ball redBall;
    public Texture2D[] textures;

    private Vector4 Yellow = new Vector4(255, 255, 0, 255);
    private Vector4 Red = new Vector4(255, 0, 0, 255);
    private Vector4 Black = new Vector4(0, 0, 0, 255);
    private Vector4 White = new Vector4(255, 255, 255, 255);
    private Vector4 Trans = new Vector4(255, 255, 255, 0);

    [Header("Camera Controller")]
    public GameObject pivotCamera;

    [Header("Effects")]
    public ParticleSystem fireWork;

    [Header("UI")]
    public Text levelText;
    public Text swipeAmountText;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        MMVibrationManager.iOSInitializeHaptics();

        instance = this;
    }

    private void Start()
    {
        levelGame = PlayerPrefs.GetInt("levelGame");
        levelText.text = "LEVEL " + (levelGame + 1);

        levelFixed = levelGame;

        if(levelFixed >= textures.Length)
        {
            levelFixed = Random.Range(0, textures.Length);
        }

        GenerateLevel();
    }

    private void LevelUp()
    {
        levelGame++;
        levelText.text = "LEVEL " + (levelGame + 1);
        PlayerPrefs.SetInt("levelGame", levelGame);

        levelFixed = levelGame;

        if (levelFixed >= textures.Length)
        {
            levelFixed = Random.Range(0, textures.Length);
        }
    }

    public void Swipe(Vector3 direction)
    {
        if (yellowBall.isMoving || redBall.isMoving || isComplete) return;

        swipeAmount++;
        swipeAmountText.text = swipeAmount.ToString();

        yellowBall.Move(direction);
        redBall.Move(direction);
    }

    private void ClearMap()
    {
        swipeAmount = 0;
        swipeAmountText.text = swipeAmount.ToString();
        isComplete = false;
        paintAmount = maxPaintAmount = 0;
        fireWork.Stop();
        PoolManager.instance.RefreshItem(PoolManager.NameObject.brick);
        PoolManager.instance.RefreshItem(PoolManager.NameObject.tile);
    }

    public void GenerateLevel()
    {
        GenerateMap(textures[levelFixed]);
    }

    private void GenerateMap(Texture2D texture)
    {
        ClearMap();

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                GenerateTile(texture, x, y);
            }
        }
    }

    private void GenerateTile(Texture2D texture, int x, int y)
    {
        Color32 pixelColor = texture.GetPixel(x, y);
        Vector4 color32 = new Vector4(pixelColor.r, pixelColor.g, pixelColor.b, pixelColor.a);

        Vector3 pos = new Vector3(x, 0, y);

        if (color32 == Red)
        {
         //   GenerateRedBal(pos);
          //  GenerateBrick(pos);
        }
        else if (color32 == Yellow)
        {
          //  GenerateYellowBall(pos);
          //  GenerateBrick(pos);
        }
        else if (color32 == White || color32 == Trans)
        {
            GenerateTile(pos);
        }
        else if (color32 == Black)
        {

        }
        else
        {
            GenerateTile(pos);
        }
        GenerateBrick(pos);
        GenerateBallTest();
    }

    private void GenerateBallTest()
    {
        Vector3 redPos = new Vector3(7, 0, 8);
        GenerateRedBal(redPos);
    //    GenerateBrick(redPos);

        Vector3 yellowPos = new Vector3(11, 0, 8);
        GenerateYellowBall(yellowPos);
    //    GenerateBrick(yellowPos);
    }

    private void GenerateYellowBall(Vector3 _position)
    {
        yellowBall.Init(_position);
    }

    private void GenerateRedBal(Vector3 _position)
    {
        redBall.Init(_position);
    }

    private void GenerateTile(Vector3 _position)
    {
        GameObject tileObject = PoolManager.instance.GetObject(PoolManager.NameObject.tile);
        tileObject.transform.position = _position;
        tileObject.SetActive(true);
    }

    private void GenerateBrick(Vector3 _position)
    {
        GameObject brickObject = PoolManager.instance.GetObject(PoolManager.NameObject.brick);
        _position.y = -0.9f;
        brickObject.transform.position = _position;
        brickObject.SetActive(true);

        maxPaintAmount++;
    }

    public void CheckComplete()
    {
        if(paintAmount >= maxPaintAmount)
        {
            Complete();
        }
    }

    private void Complete()
    {
        if (isComplete) return;

        isComplete = true;
        StartCoroutine(C_Complete());
    }

    private IEnumerator C_Complete()
    {
        fireWork.Play();
        LevelUp();
        Debug.Log("Complete");

        yield return new WaitForSeconds(0.35f);

        UIManager.instance.Show_CompleteUI();

    }

    private void Fail()
    {
        if (isComplete) return;

        isComplete = true;
        StartCoroutine(C_Fail());
    }

    private IEnumerator C_Fail()
    {
        Debug.Log("Fail");

        yield return null;
    }

    public void RedExplosion(Vector3 _pos)
    {
        GameObject obj = PoolManager.instance.GetObject(PoolManager.NameObject.redExplosion);
        obj.transform.position = _pos;
        obj.SetActive(true);
    }

    public void YellowExplosion(Vector3 _pos)
    {
        GameObject obj = PoolManager.instance.GetObject(PoolManager.NameObject.yellowExplosion);
        obj.transform.position = _pos;
        obj.SetActive(true);
    }

    private bool isVibrating;

    public void Vibration()
    {
        if (isVibrating) return;

        StartCoroutine(C_Vibration());
    }

    private IEnumerator C_Vibration()
    {
        MMVibrationManager.iOSTriggerHaptics(HapticTypes.HeavyImpact);

        isVibrating = true;

        yield return new WaitForSeconds(0.01f);

        isVibrating = false;
    }
}
