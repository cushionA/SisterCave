using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCControllerAbillity : ControllAbillity
{


    /// <summary>
    /// �G�����������ɌĂяo�����\�b�h
    /// </summary>
    public abstract void FindEnemy();


    /// <summary>
    /// ���������I�u�W�F�N�g��񍐂���
    /// </summary>
    /// <param name="isDanger">�댯�����ǂ���</param>
    /// <param name="obj">����������</param>
    public abstract void ReportObject(bool isDanger,GameObject obj);

}
