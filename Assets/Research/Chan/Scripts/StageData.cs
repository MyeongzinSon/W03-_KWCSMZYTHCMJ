using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Research.Chan;
using UnityEngine;

public class StageData
{
    // 몇 번째 스테이지인가?
    public int levelIdx;
    // 스테이지 위치 인덱스 (0 ~ maxStageCount - 1)
    public int stagePosIdx;
    // 스테이지의 크기 (int x int)
    public Vector2Int stageSize;

    // 스테이지 타일의 시작 위치 (왼쪽 아래부터)
    private Vector2Int stageStartPos;
    // 스테이지 들어가는 위치
    private Vector2Int stageEnterPos;
    // 스테이지 들어가는 사이트 크기 (int x int)
    private Vector2Int stageEnterSize;
    // 스테이지 클리어 위치
    private Vector2Int stageClearPos;
    // 스테이지 클리어 사이트 크기 (int x int)
    private Vector2Int stageClearSize;

    // 스테이지 타일의 위치 리스트
    private List<Vector2Int> _stageTilePosList;
    private bool _isStageTilePosListInitialized = false;
    // 스테이지 클리어 타일의 위치 리스트
    private List<Vector2Int> _stageClearTilePosList;
    private bool _isStageClearTilePosListInitialized = false;
    // 스테이지 진입 위치 타일의 위치 리스트
    private List<Vector2Int> _stageEnterTilePosList;
    private bool _isStageEnterTilePosListInitialized = false;

    /// <summary>
    /// 스테이지 데이터 생성자
    /// </summary>
    /// <param name="stageIdx">몇 번째 스테이지인가?</param>
    /// <param name="stageQuadrant">스테이지 사분면 위치 (직교 좌표계 기준)</param>
    /// <param name="stageSize">스테이지의 크기 (int x int)</param>
    /// <param name="stageClearPos">스테이지 클리어 위치 (왼쪽 아래부터)</param>
    /// <param name="isClearHorizontal">스테이지 클리어 방향 (가로/세로)</param>
    public StageData(int levelIdx, int stagePosIdx, Vector2Int stageSize, StageManager.StageLayout stageLayout)
    {
        this.levelIdx = levelIdx;
        this.stagePosIdx = stagePosIdx;
        this.stageSize = stageSize;
        this.stageStartPos = new Vector2Int(-((stageSize.x / 2) + 1), 0);

        if (stageLayout == StageManager.StageLayout.TRIPLE) {
            this.stageEnterPos = new Vector2Int(stageStartPos.x, stageStartPos.y + 2);
            this.stageEnterSize = new Vector2Int(1, 2);

            this.stageClearPos = new Vector2Int(stageStartPos.x + stageSize.x + 1, stageStartPos.y + 2);
            this.stageClearSize = new Vector2Int(1, 2);
        } else {
            switch (stagePosIdx) {
                case 0:
                    this.stageEnterPos = new Vector2Int(stageStartPos.x + 2, stageStartPos.y + 1);
                    this.stageEnterSize = new Vector2Int(1, 1);

                    this.stageClearPos = new Vector2Int(stageStartPos.x + stageSize.x + 1, stageStartPos.y + stageSize.y - 2);
                    this.stageClearSize = new Vector2Int(1, 2);
                    break;
                case 1:
                    this.stageEnterPos = new Vector2Int(stageStartPos.x, stageStartPos.y + stageSize.y - 2);
                    this.stageEnterSize = new Vector2Int(1, 2);

                    this.stageClearPos = new Vector2Int(stageStartPos.x + stageSize.x - 1, stageSize.y);
                    this.stageClearSize = new Vector2Int(1, 1);
                    break;
                case 2:
                    this.stageEnterPos = new Vector2Int(stageStartPos.x + stageSize.x - 1, stageStartPos.y + stageSize.y + 1);
                    this.stageEnterSize = new Vector2Int(1, 1);

                    this.stageClearPos = new Vector2Int(stageStartPos.x, stageStartPos.y + 2);
                    this.stageClearSize = new Vector2Int(1, 2);
                    break;
                case 3:
                    this.stageEnterPos = new Vector2Int(stageStartPos.x + stageSize.x + 1, stageStartPos.y + 2);
                    this.stageEnterSize = new Vector2Int(1, 2);

                    this.stageClearPos = new Vector2Int(stageStartPos.x + 2, stageStartPos.y + stageSize.y + 1);
                    this.stageClearSize = new Vector2Int(1, 1);
                    break;
            }
        }
    }

    public List<Vector2Int> StageTilePosList() {
        if (!_isStageTilePosListInitialized)
        {
            GenerateStageTilePosList();
        }
        return _stageTilePosList;
    }

    public List<Vector2Int> StageClearTilePosList() {
        if (!_isStageClearTilePosListInitialized)
        {
            GenerateStageClearTilePosList();
        }
        return _stageClearTilePosList;
    }

    public List<Vector2Int> StageEnterTilePosList() {
        if (!_isStageEnterTilePosListInitialized)
        {
            GenerateStageEnterTilePosList();
        }
        return _stageEnterTilePosList;
    }

    private void GenerateStageTilePosList() {
        if (!_isStageClearTilePosListInitialized) {
            GenerateStageClearTilePosList();
            GenerateStageEnterTilePosList();
        }

        List<Vector2Int> stageTilePosList = new List<Vector2Int>();
        for (int i = 0; i < stageSize.x + 2; i++)
        {
            for (int j = 0; j < stageSize.y + 2; j++)
            {
                Vector2Int tilePos = new Vector2Int(stageStartPos.x + i, stageStartPos.y + j);
                if (!_stageClearTilePosList.Contains(tilePos) && !_stageEnterTilePosList.Contains(tilePos)) {
                    stageTilePosList.Add(tilePos);
                }
            }
        }
        _stageClearTilePosList = stageTilePosList;
        _isStageTilePosListInitialized = true;
    }

    private void GenerateStageClearTilePosList() {
        if (_isStageClearTilePosListInitialized) {
            return;
        }

        List<Vector2Int> stageClearTilePosList = new List<Vector2Int>();
        for (int i = 0; i < stageClearSize.x; i++)
        {
            for (int j = 0; j < stageClearSize.y; j++)
            {
                stageClearTilePosList.Add(new Vector2Int(stageClearPos.x + i, stageClearPos.y + j));
            }
        }
        _stageClearTilePosList = stageClearTilePosList;
        _isStageClearTilePosListInitialized = true;
    }

    private void GenerateStageEnterTilePosList() {
        if (_isStageEnterTilePosListInitialized) {
            return;
        }

        List<Vector2Int> stageEnterTilePosList = new List<Vector2Int>();
        for (int i = 0; i < stageEnterSize.x; i++)
        {
            for (int j = 0; j < stageEnterSize.y; j++)
            {
                stageEnterTilePosList.Add(new Vector2Int(stageEnterPos.x + i, stageEnterPos.y + j));
            }
        }
        _stageEnterTilePosList = stageEnterTilePosList;
        _isStageEnterTilePosListInitialized = true;
    }
}
