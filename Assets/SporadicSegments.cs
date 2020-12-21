using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;
public class SporadicSegments : MonoBehaviour
{
    public KMSelectable[] selectables;
    public SpriteRenderer[] segments;
    public Sprite[] sprites;
    public TextMesh[] texts;
    bool[][] digits = new bool[][]
    {
        new bool[] {true, true, true, false, true, true, true},
        new bool[] {false, false, true, false, false, true, false},
        new bool[] {true, false, true, true, true, false, true},
        new bool[] {true, false, true, true, false, true, true},
        new bool[] {false, true, true, true, false, true, false},
        new bool[] {true, true, false, true, false, true, true},
        new bool[] {true, true, false, true, true, true, true},
        new bool[] {true, false, true, false, false, true, false},
        new bool[] {true, true, true, true, true, true, true},
        new bool[] {true, true, true, true, false, true, true}
    };
    List<int> digitSequence;
    int digitPointer;
    int[] segementNumbers = new int[7];
    int[] validDigits = new int[7];
    bool[,] bitmasks = new bool[7, 10];
    bool[] segmentsPressed = new bool[7];
    int lastDigit;
    public KMBombModule module;
    public KMBombInfo bomb;
    public KMAudio sound;
    int moduleId;
    static int moduleIdCounter = 1;
    bool solved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        for (int i = 0; i < 7; i++)
        {
            int j = i;
            selectables[j].OnInteract += delegate { PressSegement(j); return false; };
            selectables[j].OnHighlight += delegate { segments[j].sprite = sprites[7 + j]; };
            selectables[j].OnHighlightEnded += delegate { segments[j].sprite = sprites[j]; };
        }
        module.OnActivate += delegate { GenerateModule(); };
    }
    // I'll give you one guess why this comment is here.
    void GenerateModule()
    {
        digitSequence = Enumerable.Range(0, 10).ToList().Shuffle().Take(7).ToList();
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (rnd.Range(0, 2) == 1)
                {
                    segementNumbers[i] += (int)Math.Pow(2, j);
                    bitmasks[i, j] = true;
                }
            }
        }
        int output = 0;
        StartCoroutine(DisplayFlash());
        for (int i = 0; i < 7; i++)
        {
            output = segementNumbers[i] % 10;
            while (!digitSequence.Contains(output))
            {
                output++;
                output %= 10;
            }
            validDigits[i] = output;
        }
        Debug.LogFormat("[Sporadic Segments #{0}] The digit sequence is {1}.", moduleId, digitSequence.Join(","));
        Debug.LogFormat("[Sporadic Segments #{0}] The values of the segments are {1}.", moduleId, segementNumbers.Join(","));
        Debug.LogFormat("[Sporadic Segments #{0}] The sequence digits when the segments should be preesed, in order, are {1}.", moduleId, validDigits.Join(","));
    }

    void PressSegement(int index)
    {
        if (!solved)
        {
            Debug.LogFormat("[Sporadic Segments #{0}] You pressed segement {1} at {2}, when the display was showing {3}.", moduleId, index + 1, bomb.GetFormattedTime(), digitSequence[digitPointer]);
            if ((lastDigit == digitSequence[index] && digitSequence[digitPointer] == validDigits[index] && !segmentsPressed[index]))
            {
                segmentsPressed[index] = true;
                sound.PlaySoundAtTransform("Bweep", transform);
                Debug.LogFormat("[Sporadic Segments #{0}] That was correct.", moduleId);
                if (segmentsPressed.Count(x => x) == 7)
                {
                    module.HandlePass();
                    solved = true;
                    Debug.LogFormat("[Sporadic Segments #{0}] Module solved.", moduleId);
                    sound.PlaySoundAtTransform("Bweepbwoop", transform);
                    StartCoroutine(SolveAnim());
                }
            }
            else
            {
                Debug.LogFormat("[Sporadic Segments #{0}] That was incorrect. Strike!", moduleId);
                module.HandleStrike();
            }
        }
    }
    // Update is called once per frame
    IEnumerator DisplayFlash()
    {
        lastDigit = (int)bomb.GetTime() % 60 % 10;
        int textPointer = 0;
        string[] fluff = new string[] { "SPORADIC", "SURPRISING", "SPEEDY", "SPONTANEOUS", "SEVEN", "SEGMENT", "DISPLAY" };
        while (!solved)
        {
            while (lastDigit == (int)bomb.GetTime() % 60 % 10)
            {
                yield return null;
            }
            lastDigit = (int)bomb.GetTime() % 60 % 10;
            digitPointer++;
            if (digitPointer == 7) digitPointer = 0;
            for (int i = 0; i < 7; i++)
            {
                segments[i].enabled = digits[digitSequence[digitPointer]][i] ^ bitmasks[i, lastDigit];
            }
            textPointer++;
            if (textPointer == 7) textPointer = 0;
            StartCoroutine(TypeText(texts[0], fluff[textPointer]));
            StartCoroutine(TypeText(texts[1], fluff[(textPointer + 1) % 7]));
        }
    }
    IEnumerator SolveAnim()
    {
        bool[][] digits = new bool[][]
        {
        new bool[] {true, true, false, false, true, false, true},
        new bool[] {true, true, true, false, true, true, true},
        new bool[] {false, false, false, true, true, true, false},
        new bool[] {true, true, false, false, true, true, true},
        new bool[] {false, false, false, true, true, false, false},
        new bool[] {true, true, true, true, true, true, false},
        new bool[] {false, true, false, true, true, false, true},
        new bool[] {false, true, true, false, true, true, true},
        new bool[] {false, true, false, false, true, false, true},
        new bool[] {true, true, true, true, true, true, false},
        new bool[] {false, true, false, true, true, false, true},
        new bool[] {false, false, true, false, false, true, false},
        new bool[] {true, true, true, false, true, true, true},
        new bool[] {false, false, false, true, true, true, false},
        new bool[] {true, true, false, true, false, true, true},
        };
        int textPointer = 0;
        string[] fluff = new string[] { "SPORADIC", "SEGEMENTS", "MODULE", "BY", "PANOPTES", "BETA", "TESTING", "BY", "EXISH", "SOME", "CODE", "BY", "VFLYER", "THANKS", "FOR", "PLAYING" };
        for (int i = 0; i < 15; i++)
        {
            yield return new WaitForSeconds(0.1f);
            for (int j = 0; j < 7; j++)
            {
                segments[j].enabled = digits[i][j];
            }
            digitPointer++;
            StartCoroutine(TypeText(texts[0], fluff[textPointer]));
            StartCoroutine(TypeText(texts[1], fluff[textPointer + 1]));
            textPointer++;
        }
        yield break;
    }
    IEnumerator TypeText(TextMesh textToDisplay, string value)
    {
        yield return null;
        for (int x = 0; x < value.Length; x++)
        {
            textToDisplay.text = value.Substring(0, x + 1);
            if (solved) yield return new WaitForSeconds(0.001f);
            else yield return new WaitForSeconds(0.05f);
        }
        yield break;
    }
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <segment> at <#> <#> [Presses the specified segment when the last digit of the bomb's timer is the first number and the digit on the display is the second number] | Valid segments are T, TL, TR, M, BL, BR and B.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        string[] segmentNames = new string[] { "t", "tl", "tr", "m", "bl", "br", "b" };
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 5)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 5)
            {
                if (segmentNames.Contains(parameters[1].ToLower()) && parameters[2].ToLower().Equals("at"))
                {
                    int digit = 0;
                    int display = 0;
                    if (int.TryParse(parameters[3], out digit))
                    {
                        if (digit < 0 || digit > 9)
                        {
                            yield return "sendtochaterror The specified time to press the " + parameters[1] + " segment '" + parameters[3] + "' is out of range 0-9!";
                            yield break;
                        }
                        if (segmentsPressed[Array.IndexOf(segmentNames, parameters[1].ToLower())])
                        {
                            yield return "sendtochaterror The " + parameters[1] + " segment has already been pressed!";
                            yield break;
                        }
                        if (int.TryParse(parameters[4], out display))
                        {
                            if (digit < 0 || digit > 9)
                            {
                                yield return "sendtochaterror The specified display digit to press the " + parameters[1] + " segment '" + parameters[4] + "' is out of range 0-9!";
                                yield break;
                            }
                            if (segmentsPressed[Array.IndexOf(segmentNames, parameters[1].ToLower())])
                            {
                                yield return "sendtochaterror The " + parameters[1] + " segment has already been pressed!";
                                yield break;
                            }
                            while ((int)bomb.GetTime() % 10 != digit || digitSequence[digitPointer] != display) { yield return "trycancel Halted waiting to press due to a request to cancel!"; }
                            selectables[Array.IndexOf(segmentNames, parameters[1].ToLower())].OnInteract();
                        }
                        else
                        {
                            yield return "sendtochaterror The specified display digit to press the " + parameters[1] + " segment '" + parameters[4] + "' is invalid!";
                        }
                    }

                    else if (segmentNames.Contains(parameters[1].ToLower()) && !parameters[2].ToLower().Equals("at"))
                    {
                        yield return "sendtochaterror Invalid command format! Expected 'at' but received '" + parameters[2] + "'!";
                    }
                    else
                    {
                        yield return "sendtochaterror The specified glyph '" + parameters[1] + "' is invalid!";
                    }
                }
                else if (parameters.Length == 4)
                {
                    if (segmentNames.Contains(parameters[1].ToLower()) && parameters[2].ToLower().Equals("at"))
                    {
                        yield return "sendtochaterror Please specify at what display digit to press the " + parameters[1] + " segment!";
                    }
                    else if (segmentNames.Contains(parameters[1].ToLower()) && !parameters[2].ToLower().Equals("at"))
                    {
                        yield return "sendtochaterror Invalid command format! Expected 'at' but received '" + parameters[2] + "'!";
                    }
                    else
                    {
                        yield return "sendtochaterror The specified segment '" + parameters[1] + "' is invalid!";
                    }
                }
                else if (parameters.Length == 3)
                {
                    if (segmentNames.Contains(parameters[1].ToLower()) && parameters[2].ToLower().Equals("at"))
                    {
                        yield return "sendtochaterror Please specify when to press the " + parameters[1] + " segment!";
                    }
                    else if (segmentNames.Contains(parameters[1].ToLower()) && !parameters[2].ToLower().Equals("at"))
                    {
                        yield return "sendtochaterror Invalid command format! Expected 'at' but received '" + parameters[2] + "'!";
                    }
                    else
                    {
                        yield return "sendtochaterror The specified segment '" + parameters[1] + "' is invalid!";
                    }
                }
                else if (parameters.Length == 2)
                {
                    if (segmentNames.Contains(parameters[1].ToLower()))
                    {
                        yield return "sendtochaterror Please specify when to press the " + parameters[1] + " segment!";
                    }
                    else
                    {
                        yield return "sendtochaterror The specified segment '" + parameters[1] + "' is invalid!";
                    }
                }
                else if (parameters.Length == 1)
                {
                    yield return "sendtochaterror Please specify which segment to press and when to press it!";
                }
                yield break;
            }
        }
    }
}
