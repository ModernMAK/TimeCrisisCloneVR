using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualAmmoManager : MonoBehaviour
{
    #pragma warning disable 649
    [SerializeField]
    private Sprite _sprite;
    [SerializeField]
    private Color _spriteColor;
    
    [SerializeField] private bool _leftToRight;

    [SerializeField] private HorizontalOrVerticalLayoutGroup _group;
    private RectTransform _groupTran;

    private List<Image> _childIcons;
    #pragma warning restore 649

    
    public void ResizeIconCount(int count)
    {
        for (var i = _childIcons.Count; i < count; i++)
        {
            var go = new GameObject($"Icon {i+1}",typeof(RectTransform), typeof(Image));
            var rTran = go.transform as RectTransform;
            rTran.SetParent(_groupTran);
            rTran.localPosition = Vector3.zero;
            rTran.localRotation = Quaternion.identity;
            rTran.localScale = Vector3.one;
            var image = go.GetComponent<Image>();
            _childIcons.Add(image);
            
        }
        for (var i = 0; i < count &&  i < _childIcons.Count; i++)
        {
            _childIcons[i].sprite = _sprite;
            _childIcons[i].color = _spriteColor;
            _childIcons[i].preserveAspect = true;
            _childIcons[i].gameObject.SetActive(true);
        }
        for(var i = count; i < _childIcons.Count; i++)
            _childIcons[i].gameObject.SetActive(false);

        _group.childAlignment = _leftToRight ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
        GroupSet(true);//Fix Layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(_groupTran);
        GroupSet(false);
    }

    public void SetActive(int count)
    {
        for (var i = 0; i < _childIcons.Count; i++)
            _childIcons[i].gameObject.SetActive(i < count);

    }
    
    void GroupSet(bool value)
    {
        _group.childForceExpandWidth = value;
        _group.childForceExpandHeight = value;
        _group.childControlHeight = value;
        _group.childControlWidth = value;
    }

    private void Awake()
    {
        _childIcons = new List<Image>();
        _groupTran = _group.transform as RectTransform;
    }

}
