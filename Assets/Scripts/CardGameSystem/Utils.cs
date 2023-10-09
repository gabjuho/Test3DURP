using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

//Position, Rotation, Scale 정보가 포함되어 있는 클래스 (사실 Transform이 존재해서 딱히 의미있는 클래스일까 싶음)
[System.Serializable]
public class PRS
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;
    public PRS(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
    }
}

public class Utils
{
    public static Quaternion QI => Quaternion.identity;

    //CarData List를 받아 랜덤하게 섞고 반환해주는 함수
    public static List<CardData> ShuffleDeckData(List<CardData> deckData)
    {
        Random random = new Random();
        List<CardData> randomCardDeckData = deckData.OrderBy(_ => random.Next()).ToList();

        return randomCardDeckData;
    }

    //CardField의 AnchoredPosition을 Canvas 기준 Position으로 바꾸어주는 함수
    public static Vector3 CardFieldAnchoredPositionToCanvasPosition(Vector3 cardFieldAnchoredPosition)
    {
        cardFieldAnchoredPosition.x += 300f;
        cardFieldAnchoredPosition.y = 880f + cardFieldAnchoredPosition.y;

        return cardFieldAnchoredPosition;
    }
}
