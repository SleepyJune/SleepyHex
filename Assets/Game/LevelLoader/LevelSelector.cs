using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Amazon.S3.Model;


public class LevelSelector : MonoBehaviour
{
    public static Dictionary<string, LevelTextAsset> levelDatabase = new Dictionary<string, LevelTextAsset>();
    public static List<LevelTextAsset> levelListDatabase = new List<LevelTextAsset>();
    public static bool isLoaded = false;

    public enum SortType
    {
        DateModified,
        Name,
        Difficulty,
        ID,
    }

    public Transform levelList;
    public Transform levelListParent;

    public GameObject levelSelectionButton;

    public LevelLoader levelLoader;

    protected AmazonS3Helper amazonHelper;

    public Button[] difficultyButtons;

    public SortType sortType = SortType.Difficulty;

    [NonSerialized]
    public int difficultyFilter = -1;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        amazonHelper = AmazonS3Helper.instance;
        difficultyFilter = PlayerPrefs.GetInt("difficultyFilter", -1);
        LoadLevels();

        //WriteLevelID();
    }

    void LoadLevels()
    {
        if (!isLoaded)
        {
            LoadLevelLocal();

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                //amazonHelper.ListLevelVersions(LoadLevelListWeb);
                RefreshList();
            }
            else
            {
                RefreshList();
            }

            isLoaded = true;
        }
        else
        {
            RefreshList();
        }
    }

    void WriteLevelID()
    {
        for (int i = 2; i <= 2; i++)//for (int i = 1; i <= 4; i++)
        {
            int levelID = 1;

            IEnumerable<LevelTextAsset> filteredLevels =
                levelDatabase.Values.Where(level => Math.Floor(level.difficulty) == i);

            var comparer = new NaturalComparer();

            foreach (var levelText in filteredLevels.OrderBy(level => level.difficulty)
                                                    .ThenBy(level => level.levelName, comparer))
            {
                var level = Level.LoadLevel(levelText);
                if (level != null)
                {
                    level.levelID = levelID++;
                    level.SaveLevel(false);
                }
            }
        }
    }

    void WriteLevelDifficulty()
    {
        for (int i = 1; i <= 4; i++)
        {
            IEnumerable<LevelTextAsset> filteredLevels =
                levelDatabase.Values.Where(level => Math.Floor(level.difficulty) == i);

            var comparer = new NaturalComparer();

            foreach (var levelText in filteredLevels.OrderBy(level => level.levelName, comparer))
            {
                var level = Level.LoadLevel(levelText);
                if (level != null)
                {
                    level.difficulty += .5f;
                    level.SaveLevel(false);
                }
            }
        }
    }

    public void SetDifficultyFilter(int difficulty)
    {
        difficultyFilter = difficultyFilter == difficulty ? -1 : difficulty;
        PlayerPrefs.SetInt("difficultyFilter", difficultyFilter);
    }

    public void LoadLevelListWeb(List<LevelVersion> data)
    {
        if (data == null)
        {
            return;
        }

        foreach (var file in data)
        {
            LevelTextAsset level;
            string dateModified = file.dateModified;
            string dateCreated = file.dateCreated;

            if (levelDatabase.TryGetValue(file.levelName, out level))
            {
                level.webVersion = file.version;
                level.dateModified = dateModified;
                level.webVersionFile = file;
                level.difficulty = file.difficulty;
                level.levelID = file.levelID;
            }
            else
            {
                var levelTextAsset = new LevelTextAsset(file.levelName, -1, file.version, dateModified, dateCreated);

                levelTextAsset.webVersionFile = file;
                levelTextAsset.difficulty = file.difficulty;
                levelTextAsset.levelID = file.levelID;

                AddLevel(levelTextAsset);
            }
        }

        Debug.Log("Downloaded file list: " + data.Count);

        RefreshList();
    }

    void LoadLevelLocal()
    {
        if (!Directory.Exists(DataPath.savePath))
        {
            Directory.CreateDirectory(DataPath.savePath);
        }

        if (Application.platform != RuntimePlatform.WindowsEditor) //overwrite files if mobile platform
        {
            LoadLevelFromAsset();
        }
        else
        {
            LoadLevelFromFile();
        }
    }

    void LoadLevelFromString(string str)
    {
        var levelTextAsset = new LevelTextAsset("new", 0, -1, DateTime.UtcNow.ToString(), DateTime.UtcNow.ToString());
        levelTextAsset.text = str;

        Level level = Level.LoadLevel(levelTextAsset);
        if (level != null)
        {
            levelTextAsset.levelName = level.levelName;
            levelTextAsset.levelID = level.levelID;
            levelTextAsset.localVersion = level.version;
            levelTextAsset.hasSolution = level.hasSolution;
            levelTextAsset.difficulty = level.difficulty;

            levelTextAsset.dateCreated = level.dateCreated;
            levelTextAsset.dateModified = level.dateModified;

            if (level.hasSolution)
            {
                if (level.solution.bestScore == level.solution.worstScore
                    && level.solution.numBestSolutions != level.solution.numSolutions)
                {
                    levelTextAsset.hasSolution = false;
                }
            }

            AddLevel(levelTextAsset);

            //upload versions
            /*LevelVersion version = new LevelVersion()
            {
                levelName = level.levelName,
                levelID = level.levelID,
                version = level.version,
                solved = level.hasSolution,
                difficulty = level.difficulty,
                dateCreated = level.dateCreated,
                dateModified = levelTextAsset.dateModified.ToString(),

            };

            amazonHelper.UploadLevelVersion(version);*/
        }
    }

    void LoadLevelFromAsset()
    {
        var levels = Resources.LoadAll("Levels", typeof(TextAsset));

        foreach (var obj in levels)
        {
            var asset = obj as TextAsset;

            //var path = DataPath.savePath + level.name + ".json";
            //File.WriteAllText(DataPath.savePath + level.name + ".json", level.text);

            LoadLevelFromString(asset.text);
        }
    }

    void LoadLevelFromFile()
    {
        var fileListPath = DataPath.savePath + DataPath.fileListFolder + DataPath.fileListName;

        DirectoryInfo d = new DirectoryInfo(DataPath.savePath);
        foreach (var file in d.GetFiles("*.json"))
        {
            var path = file.FullName;
            string str = File.ReadAllText(path);
            
            LoadLevelFromString(str);
        }
    }

    void AddButton(LevelTextAsset levelText)
    {
        var newButton = Instantiate(levelSelectionButton, levelList);        
        newButton.GetComponent<Button>().onClick.AddListener(() => LoadLevel(levelText.levelName));

        if(sortType == SortType.ID)
        {
            newButton.GetComponentInChildren<Text>().text = levelText.levelID.ToString();
        }
        else
        {
            newButton.GetComponentInChildren<Text>().text = levelText.levelName;
        }

        if (levelText.webVersion > levelText.localVersion ||
            (levelText.webVersionFile != null && !levelText.hasSolution && levelText.webVersionFile.solved))
        {
            newButton.transform.Find("Panel").gameObject.SetActive(true);
        }

        if (!levelText.hasSolution)
        {
            newButton.transform.Find("Unsolved").gameObject.SetActive(true);
        }

        if (levelText.difficulty > 0)
        {
            PuzzleDifficulty difficulty = (PuzzleDifficulty)levelText.difficulty;

            var ratingTransform = newButton.transform.Find("Rating");
            ratingTransform.gameObject.SetActive(true);
            ratingTransform.Find("Text").GetComponent<Text>().text = levelText.difficulty.ToString();
        }

    }

    public void SetSortType(int sortType)
    {
        this.sortType = (SortType)sortType;
        RefreshList();
    }

    public void RefreshList()
    {        
        foreach (Transform child in levelList)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            if (difficultyFilter == -1)
            {
                difficultyButtons[i].gameObject.SetActive(true);
            }
            else
            {
                if (difficultyFilter - 1 != i)
                {
                    difficultyButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    difficultyButtons[i].gameObject.SetActive(true);
                }
            }
        }

        IEnumerable<LevelTextAsset> filteredLevels =
            difficultyFilter > 0 ? levelDatabase.Values.Where(level => Math.Floor(level.difficulty) == difficultyFilter) : levelDatabase.Values;

        if (sortType == SortType.Difficulty)
        {
            var comparer = new NaturalComparer();
            levelListDatabase = filteredLevels
                        .OrderByDescending(level => level.difficulty)
                        .ThenByDescending(level => level.levelName, comparer)
                        .ToList();
        }
        else if (sortType == SortType.Name)
        {
            var comparer = new NaturalComparer();
            levelListDatabase = filteredLevels.OrderByDescending(level => level.levelName, comparer).ToList();

            //levelListDatabase = filteredLevels.OrderByDescending(level => level.levelID).ToList();
        }
        else if(sortType == SortType.ID)
        {
            levelListDatabase = filteredLevels.OrderByDescending(level => level.levelID).ToList();
        }
        else
        {
            levelListDatabase = filteredLevels.OrderByDescending(level => DateTime.Parse(level.dateModified)).ToList();
        }

        Debug.Log("Num levels: " + levelListDatabase.Count());

        foreach (var level in levelListDatabase)
        {
            if (level != null)
            {
                AddButton(level);
            }
        }
    }

    public LevelTextAsset GetLevel(string name)
    {
        LevelTextAsset levelText;
        if (levelDatabase.TryGetValue(name, out levelText))
        {
            return levelText;
        }

        return null;
    }

    public void LoadLevel(string name)
    {
        LevelTextAsset levelText;
        if (levelDatabase.TryGetValue(name, out levelText))
        {
            if (levelText.webVersion > levelText.localVersion ||
            (levelText.webVersionFile != null && !levelText.hasSolution && levelText.webVersionFile.solved))
            {
                var filename = DataPath.webPath + levelText.levelName + ".json";
                amazonHelper.GetFile(filename, name, LoadLevelTextWeb);
            }
            else
            {
                var filePath = DataPath.savePath + levelText.levelName + ".json";
                if (File.Exists(filePath))
                {
                    string data = File.ReadAllText(filePath);
                    levelText.text = data;

                    levelLoader.Load(levelText);
                }
                else
                {
                    levelText.localVersion = -1;
                    return;
                }
                levelListParent.gameObject.SetActive(false);
            }
        }
    }

    public void LoadLevelTextWeb(string name, string data)
    {
        if (data == null)
        {
            return;
        }

        LevelTextAsset levelText;
        if (levelDatabase.TryGetValue(name, out levelText))
        {
            levelText.text = data;

            Level level = levelLoader.Load(levelText);
            level.SaveLevel(false);

            levelText.localVersion = levelText.webVersion;
            levelText.dateModified = level.dateModified;

            levelListParent.gameObject.SetActive(false);
            RefreshList();
        }
    }

    public static void AddLevel(LevelTextAsset newLevel, bool overwrite = false)
    {
        if (levelDatabase.ContainsKey(newLevel.levelName))
        {
            if (overwrite)
            {
                levelDatabase[newLevel.levelName] = newLevel;
            }
        }
        else
        {
            levelDatabase.Add(newLevel.levelName, newLevel);
        }
    }

    public static void DeleteLevel(string name)
    {
        if (levelDatabase.ContainsKey(name))
        {
            levelDatabase.Remove(name);
        }
    }

    public void ShowLevelList()
    {
        levelListParent.gameObject.SetActive(true);
    }

    public void HideLevelList()
    {
        levelListParent.gameObject.SetActive(false);
    }
}
