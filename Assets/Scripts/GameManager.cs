using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public int totalTrees;
    public int treesOnFire;
    public int burnedTrees;

    public RectTransform total, burning, burned;

    public GameObject newGame;
    public GameObject gameOver;
    bool over = false;

    float gameStartedAt;

    public void RegisterTree() {
        totalTrees++;
    }

    public void TreeOnFire() {
        treesOnFire++;
    }

    public void TreeBurned() {
        treesOnFire--;
        burnedTrees++;
    }

    private void Start() {
        gameStartedAt = Time.time;
        gameOver.SetActive(false);
        StartCoroutine(NewGame());
    }

    private void Update() {
        if (over) return;
        if (burnedTrees >= totalTrees) {
            Win();
        }

        if (Time.time - gameStartedAt > 10.0f && treesOnFire <= 0) {
            GameOver();
        }

        UpdateScore();
    }


    IEnumerator NewGame() {
        newGame.SetActive(true);
        Text t = newGame.GetComponent<Text>();
        yield return new WaitForSeconds(3.0f);
        float fadeDuration = 1.0f;
        float elapsed = 0.0f;
        while (elapsed < fadeDuration) {
            elapsed += Time.deltaTime;
            t.color = new Color(t.color.r, t.color.g, t.color.b, Mathf.Lerp(1.0f, 0.0f, elapsed / fadeDuration));
            yield return null;
        }

        newGame.SetActive(false);
    }


    void GameOver() {
        float score = (float)burnedTrees / (float)totalTrees;
        Debug.Log("Game over, score: " + score);
        gameOver.transform.Find("score").GetComponent<Text>().text = string.Format("Trees burned down: {0:0.00%}", score);
        gameOver.SetActive(true);
        over = true;
    }

    void Win() {
        Debug.Log("win");
    }

    void UpdateScore() {
        float width = total.rect.width;

        float burningWidth = (burnedTrees + treesOnFire) * (width / totalTrees);
        burning.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, burningWidth);

        float burnedWidth = (burnedTrees) * (width / totalTrees);
        burned.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, burnedWidth);
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
