using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InspectDialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public MaskableGraphic dialogueBackground; // works with Image or RawImage

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.03f;
    public int wordsPerChunk = 1000;
    public Sprite boxSprite;

    [Header("Pop Effect Settings")]
    public float popScale = 1.25f;
    public float popDuration = 0.05f;

    [Header("Sound Effect (Optional)")]
    public AudioSource blipSource;
    public AudioClip blipSound;

    [Header("Slide Animation Settings")]
    public float slideDuration = 0.4f;  // time to move in/out
    public float slideDistance = 300f;  // how far offscreen it starts

    private Queue<string> textChunks;
    private bool isTyping = false;
    private bool dialogueActive = false;
    private RectTransform boxRect;
    private Vector2 originalAnchoredPos;

    void Awake()
    {
        dialogueBox.SetActive(false);
        textChunks = new Queue<string>();
        boxRect = dialogueBox.GetComponent<RectTransform>();
        originalAnchoredPos = boxRect.anchoredPosition;
    }

    // --- Start Dialogue ---
    public void StartDialogue(string fullText)
    {
        dialogueActive = true;
        dialogueBox.SetActive(true);
        textChunks.Clear();

        // Set background sprite (if assigned)
        if (dialogueBackground is Image img && boxSprite != null)
            img.sprite = boxSprite;
        else if (dialogueBackground is RawImage raw && boxSprite != null)
            raw.texture = boxSprite.texture;

        // Split text
        string[] words = fullText.Split(' ');
        List<string> chunks = new List<string>();
        for (int i = 0; i < words.Length; i += wordsPerChunk)
        {
            int length = Mathf.Min(wordsPerChunk, words.Length - i);
            chunks.Add(string.Join(" ", words, i, length));
        }
        foreach (string chunk in chunks)
            textChunks.Enqueue(chunk);

        StopAllCoroutines();
        StartCoroutine(SlideInThenStart());
    }

    IEnumerator SlideInThenStart()
    {
        yield return StartCoroutine(SlideIn());
        yield return StartCoroutine(DisplayNextChunk());
    }

    // --- Slide In / Out Animations ---
    IEnumerator SlideIn()
    {
        Vector2 startPos = originalAnchoredPos - new Vector2(0, slideDistance);
        Vector2 endPos = originalAnchoredPos;
        boxRect.anchoredPosition = startPos;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / slideDuration;
            boxRect.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        boxRect.anchoredPosition = endPos;
    }

    IEnumerator SlideOut()
    {
        Vector2 startPos = originalAnchoredPos;
        Vector2 endPos = originalAnchoredPos - new Vector2(0, slideDistance);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / slideDuration;
            boxRect.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        dialogueBox.SetActive(false);
    }

    // --- Display Text ---
    IEnumerator DisplayNextChunk()
    {
        while (textChunks.Count > 0)
        {
            string chunk = textChunks.Dequeue();
            yield return StartCoroutine(TypeText(chunk));

            // small delay before wipe
            yield return new WaitForSeconds(0.6f);
            yield return StartCoroutine(FadeOutText(0.3f));
            yield return new WaitForSeconds(0.3f);
        }

        yield return StartCoroutine(SlideOut());
        dialogueActive = false;
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        dialogueText.ForceMeshUpdate();

        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.text += text[i];
            dialogueText.ForceMeshUpdate();

            // sound
            if (blipSource && blipSound && i % 2 == 0)
                blipSource.PlayOneShot(blipSound, Random.Range(0.4f, 0.6f));

            // pop
            StartCoroutine(PopLetterEffect(i));

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    // --- Pop effect for letters ---
    IEnumerator PopLetterEffect(int index)
    {
        TMP_TextInfo textInfo = dialogueText.textInfo;
        dialogueText.ForceMeshUpdate();

        if (index >= textInfo.characterCount) yield break;

        var charInfo = textInfo.characterInfo[index];
        if (!charInfo.isVisible) yield break;

        int vertexIndex = charInfo.vertexIndex;
        int materialIndex = charInfo.materialReferenceIndex;
        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

        Vector3 center = (vertices[vertexIndex] +
                          vertices[vertexIndex + 1] +
                          vertices[vertexIndex + 2] +
                          vertices[vertexIndex + 3]) / 4f;

        for (float t = 0; t < 1f; t += Time.deltaTime / popDuration)
        {
            float scale = Mathf.Lerp(popScale, 1f, t);
            for (int j = 0; j < 4; j++)
                vertices[vertexIndex + j] = center + (vertices[vertexIndex + j] - center) * scale;

            dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }

        dialogueText.ForceMeshUpdate();
    }

    // --- Fade Out ---
    IEnumerator FadeOutText(float duration)
    {
        float startAlpha = 1f;
        float time = 0f;
        Color color = dialogueText.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, time / duration);
            dialogueText.color = color;
            yield return null;
        }

        color.a = 0f;
        dialogueText.color = color;
        dialogueText.text = "";
        color.a = 1f;
        dialogueText.color = color;
    }
}
