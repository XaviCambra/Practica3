using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IScoreManager
{
    void addPoints(float f);
    float getPoints();
    event ScoreChanged scoreChangedDelegate;
}
public delegate void ScoreChanged(IScoreManager scoreManager);
public class ScoreManager : MonoBehaviour, IScoreManager
{
    [SerializeField] float points;
    public event ScoreChanged scoreChangedDelegate;
    void Awake()
    {
        DependencyInjector.AddDependency<IScoreManager>(this);
    }
    public void addPoints(float points)
    {
        this.points += points;
        scoreChangedDelegate?.Invoke(this);
    }
    public float getPoints() { return points; }
}
