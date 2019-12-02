using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StringValue
{
  public string name;
  string _value;

  bool _valueInitial = true;
  public string Value
  {
    get { return _value; }
    set
    {
      if( value != _value || _valueInitial )
      {
        _value = value;
        _valueInitial = false;
        if( onValueChanged != null )
          onValueChanged.Invoke( _value );
        // check for differing value to avoid infinite loop 
        updateView( _value );
      }
    }
  }

  public System.Action<string> updateView;
  public System.Action<string> onValueChanged;

  public Text labelText;
  public Text valueText;
  public InputField inputField;

  public void Init()
  {
    if( labelText != null && labelText.text == "<name>" )
      labelText.text = name;

    updateView = new System.Action<string>( delegate ( string value )
    {
      if( inputField != null )
        inputField.text = value;
      if( valueText != null )
        valueText.text = value;
    } );
  }
}

[System.Serializable]
public class FloatValue
{
  public string name;
  float _value;
  bool _valueInitial = true;
  public float Value
  {
    get { return _value; }
    set
    {
      if( value != _value || _valueInitial )
      {
        _value = value;
        _valueInitial = false;
        if( onValueChanged != null )
          onValueChanged.Invoke( _value );
        // check for differing value to avoid infinite loop 
        updateView( _value );
      }
    }
  }

  public System.Action<float> updateView;
  public System.Action<float> onValueChanged;

  public Text labelText;
  public Text valueText;
  public Slider slider;

  public void Init()
  {
    if( labelText != null && labelText.text == "<name>" )
      labelText.text = name;

    updateView = new System.Action<float>( delegate ( float value )
    {
      if( slider != null )
        slider.value = value;
      if( valueText != null )
        valueText.text = value.ToString( "0.##" );
    } );

    if( slider != null )
    {
      slider.onValueChanged.AddListener( new UnityEngine.Events.UnityAction<float>( delegate ( float value )
      {
        Value = value;
      } ) );
    }
  }
}

[System.Serializable]
public class BoolValue
{
  public string name;
  bool _value;
  bool _valueInitial = true;
  public bool Value
  {
    get { return _value; }
    set
    {
      if( value != _value || _valueInitial )
      {
        _value = value;
        _valueInitial = false;
        if( onValueChanged != null )
          onValueChanged.Invoke( _value );
        //onValueChanged( _value );
        // check for differing value to avoid infinite loop 
        updateView( _value );
      }
    }
  }

  public System.Action<bool> updateView;
  public System.Action<bool> onValueChanged;

  public Text labelText;
  public Text valueText;
  public Toggle toggle;

  public void Init()
  {
    if( labelText != null && labelText.text == "<name>" )
      labelText.text = name;

    updateView = new System.Action<bool>( delegate ( bool value )
    {
      if( toggle != null )
        toggle.isOn = value;
      if( valueText != null )
      {
        valueText.text = value.ToString();
      }
    } );

    if( toggle != null )
    {
      toggle.onValueChanged.AddListener( new UnityEngine.Events.UnityAction<bool>( delegate ( bool value )
      {
        Value = value;
      } ) );
    }
  }
}

public class SettingUI : MonoBehaviour
{
  public bool isInteger = false;
  public FloatValue intValue;
  public bool isBool = false;
  public BoolValue boolValue;
  public bool isString;
  public StringValue stringValue;
}
