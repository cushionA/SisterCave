using EnhancedUI.EnhancedScroller;
using UnityEngine;
using EnhancedUI;
using System;

public class UseController : MonoBehaviour, IEnhancedScrollerDelegate
{
    public EnhancedScroller m_scroller;
    public FooCellView m_cellPrefab;

    private UseCellData[] m_list;

    public Sprite tes1;
    public Sprite tes2;
    public Sprite tes3;

    private void Start()
    {
        m_list = new[]
        {
            new UseCellData { m_sprite1 = tes1    },
            new UseCellData { m_sprite2 = tes2   },
            new UseCellData { m_sprite3 = tes3    },


        };

        m_scroller.Delegate = this;
        m_scroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return m_list.Length;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 60f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var CellView = scroller.GetCellView(m_cellPrefab) as FooCellView;
        //CellView.SetData(m_list[dataIndex]);
        return CellView;
    }

}