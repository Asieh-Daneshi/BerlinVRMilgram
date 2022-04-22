using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
#endif


public class ExperimentConfigNew
{
    public ExperimentConfigNew()
    {
        positions = new List<float[]>();
        blocks = new List<Block>();
    }

    public int condition;
    public string participant;
    public int time_exp_ms;
    public double soa_sd_ms;
    public List<float[]> positions;

    public void addPosition(float[] p)
    {
        positions.Add(p);
    }

    public List<Block> blocks;

    public void addBlock(Block b)
    {
        blocks.Add(b);
    }

    public List<float[]> getPositions()
    {
        return positions;
    }
}

public class Block
{
    public Block()
    {
        trials = new List<TrialData>();
    }

    public List<TrialData> trials;

    public void addTrial(TrialData t)
    {
        trials.Add(t);
    }
}

[Serializable]
public class TrialData
{
    public int time_blank_ms;
    public double soa_means_ms;
    public int gaze_loc;

    public int audio_cue;

    //    public List<int> characters_gazing;
    public int[] characters_gazing;

    public void PrintIt()
    {
        Debug.Log("audio cue: " + audio_cue + ", " + "gaze_loc: " + gaze_loc + ", ");
    }
}