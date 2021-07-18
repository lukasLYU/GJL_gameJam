using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class headBobControll : MonoBehaviour
{
    public bool _enable = true;
    public float _amplitude = 0.015f;
    public float _frequency = 10.0f;
    public Transform _cam = null;
    public Transform _camHolder = null;
    public float _toggleSpeed = 3f;
    public Vector3 _startPos;
    public playerController _controller;
    void Awake()
    {
        _controller = GetComponent<playerController>();
        _startPos = _cam.localPosition;
    }
    void Update()
    {
        
    }
    private Vector3 FootStepMotion() {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
        pos.x += Mathf.Cos(Time.time * _frequency / 2) * _amplitude * 2;
        return pos;
    }
    private void CheckMotion() {
        float speed = new Vector3(_controller.body.velocity.x, 0, _controller.body.velocity.z).magnitude;
        if (speed < _toggleSpeed) return;

        PlayMotion(FootStepMotion());
    }
    private void ResetPosition(){
        if (_cam.localPosition == _startPos) return;
        _cam.localPosition = Vector3.Lerp(_cam.localPosition, _startPos, 1 * Time.deltaTime);
    }
    private void PlayMotion(Vector3 motion){
        _cam.localPosition += motion;
    }
}
