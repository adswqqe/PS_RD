using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFSMStateBase
{
    void StartState();
    void Update();
    void EndState();

}

public abstract class FSMSystem<EnumType, State> : MonoBehaviour where EnumType : System.Enum where State : IFSMStateBase
{
    [SerializeField, ReadOnly]
    private EnumType _currState;

    public EnumType CurrState => _currState;

    [SerializeField]
    private EnumType _startState;
    public EnumType StartState => _startState;

    [SerializeField, ReadOnly]
    private bool _isInit = false;
    public bool IsInit => _isInit;

    private Dictionary<EnumType, State> _fsmData = new Dictionary<EnumType, State>();

    private void Awake()
    {
        InitState();
    }

    protected abstract void RegisterState();

    private void InitState()
    {
        RegisterState();
        ChangeState(StartState);
        _isInit = true;
    }

    public void ChangeState(EnumType state)
    {
        if (_fsmData.ContainsKey(state) == false)
            return;

        if (IsInit == true)
        {
            if(_fsmData.ContainsKey(_currState) == true)
            {
                _fsmData[_currState].EndState();
            }
        }

        _currState = state;
        _fsmData[state].StartState();
    }


    // Update is called once per frame
    void Update()
    {
        if (IsInit == false)
            return;

        if (_fsmData.ContainsKey(_currState) == true)
            _fsmData[_currState].Update();
    }

    protected void AddState(EnumType type, State fsmState)
    {
        if (fsmState == null)
            return;

        if (_fsmData.ContainsKey(type) == true)
        {
            Debug.LogError("Have FSMState " + type.ToString());
            return;
        }

        _fsmData.Add(type, fsmState);
    }
}
