using UnityEngine;
using System.Collections;

public partial class Character
{
  [Header( "Converse" )]
  public bool CanConverse = true;
  public Character ConverseTarget;
  float ConverseStartTime;
  float ConverseEndTime;
  float ConverseTalkInterval = 10f;
  float ConverseLastTalkTime;
  // detect if other character has moved away after conversation has started
  bool ConversationStarted = false;

  const float ConverseRadius = 0.5f;
  const float ConverseMeetTimeout = 10;
  const float ConverseTalkTimeout = 20;
  const float ConverseCooldownDuration = 30;


  void ConsiderConverse( Interest interest )
  {
    if( !CanConverse )
      return;
    if( Time.time - ConverseEndTime < ConverseCooldownDuration )
      return;
    if( interest.go == null )
      return;
    Character cha = interest.go.GetComponent<Character>();
    if( cha == null )
      return;
    AffinityData aff = GetAffinity( cha.id );
    if( aff.Value < AffTalk )
      return;
    if( cha.RequestConversation( this ) )
    {
      ConverseTarget = cha;
      PushState( "Converse" );
    }
  }

  public bool RequestConversation( Character instigator )
  {
    if( ConverseTarget != null )
      return false;
    if( Time.time - ConverseEndTime < ConverseCooldownDuration )
      return false;
    if( CurrentState.Priority < states.Find( x => x.Name == "Converse" ).Priority )
    {
      ConverseTarget = instigator;
      PushState( "Converse" );
      return true;
    }
    //    if( isPlayerControlled )
    //      return true;
    return false;
  }

  public void CancelConversation()
  {
    if( CurrentState.Name == "Converse" )
      PopState();
  }

  void PushConverse()
  {
    ConverseStartTime = Time.time;
    CurrentMoveSpeed = WalkSpeed;
    ConversationStarted = false;
    Speak( "hello" );
  }

  void UpdateConverse()
  {
    if( ConverseTarget == null )
    {
      PopState();
      return;
    }

    if( Vector3.SqrMagnitude( ConverseTarget.moveTransform.position - moveTransform.position ) < ConverseRadius * ConverseRadius )
    {
      // within conversation radius
      if( !ConversationStarted )
        TalkShareEvent( ConverseTarget );
      ConversationStarted = true;
      CurrentMoveSpeed = 0;
      //Debug.DrawLine( moveTransform.position, ConverseTarget.moveTransform.position, Color.blue );
      if( Time.time - ConverseStartTime > ConverseTalkTimeout )
      {
        Debug.Log( "Conversation timeout, " + CharacterName + " and " + ConverseTarget.CharacterName );
        Speak( "bye" );
        PopState();
        PushState( "Wander" );
        return;
      }

      if( Time.time - ConverseLastTalkTime > ConverseTalkInterval )
      {
        ConverseLastTalkTime = Time.time;
        TalkShareEvent( ConverseTarget );
        //Speak( "<random conversation text> "+Time.time );
      }
    }
    else
    {
      // outside of conversation radius
      if( ConversationStarted )
      {
        Speak( "bye", 1 );
        PopState();
        return;
      }
      CurrentMoveSpeed = WalkSpeed;
      //Debug.DrawLine( moveTransform.position, ConverseTarget.moveTransform.position, Color.green );
      if( Time.time - ConverseStartTime > ConverseMeetTimeout )
      {
        Debug.Log( "Conversation meetup timeout, " + CharacterName + " and " + ConverseTarget.CharacterName );
        PopState();
        return;
      }
      SetPath( ConverseTarget.moveTransform.position );
    }
  }

  void PopConverse()
  {
    ConverseEndTime = Time.time;
    if( ConverseTarget != null )
    {
      ConverseTarget.ConverseTarget = null;
      ConverseTarget.CancelConversation();
      ConverseTarget = null;
    }
  }






  void TalkDirect( Character instigator )
  {
    // tell the instigator what I think about them.
    AffinityData aff = GetAffinity( instigator.id );
    if( aff.Value > AffTalk )
    {
      if( Random.Range( 0, 2 ) > 0 )
      {
        // tell instigator what I think about them
        string say = "...";
        if( aff.Value < AffPeaceful )
          say = "Die!";
        else
        if( aff.Value < 0 )
          say = "Hi. You can help out if you want.";
        else
        if( aff.Value < AffFollow )
          say = "I think you're alright.";
        else
        if( aff.Value < AffShare )
          say = "I trust you.";

        Speak( say, 2 );
      }
    }
  }

  void TalkShareEvent( Character instigator )
  {
    AffinityData aff = GetAffinity( instigator.id );

    if( aff.Value > AffPeaceful && aff.Value < AffShare - 1 )
      ModifyAffinity( instigator.id, AffModTalk );

    if( aff.Value > AffTalk )
    {       
      if( KnownEvents.Count == 0 )
      {
        Speak( "random" );
        return;
      }

      foreach( var evt in KnownEvents )
      {
        CharacterInfo cia = Global.Instance.CharacterInfoLookup[ evt.CharacterIdA ];
        CharacterInfo cib = Global.Instance.CharacterInfoLookup[ evt.CharacterIdB ];
        instigator.ReceiveEvent( evt );
        // debug
        Speak( evt.type.ToString() + " " + cia.name + " " + cib.name +" "+ (Time.time-evt.time).ToString("F0" )+" seconds ago.", 2 );
      }
    }
  }

  #if false
  int GossipIndex = 0;

  void TalkGossip( Character instigator )
  {
    AffinityData aff = GetAffinity( instigator.id );

    if( aff.Value > AffPeaceful && aff.Value < AffShare - 1 )
      ModifyAffinity( instigator.id, AffModTalk );

    if( aff.Value > AffTalk )
    {      
      if( KnownEvents.Count == 0 )
      {
        Speak( "hello" );
        return;
      }
      // choose a ConverseEvent to share with the instigator
      CharacterEvent evt = KnownEvents[ GossipIndex++ % KnownEvents.Count ];

      CharacterInfo witness = Global.Instance.CharacterInfoLookup[ evt.WitnessId ];
      CharacterInfo cia = Global.Instance.CharacterInfoLookup[ evt.CharacterIdA ];
      CharacterInfo cib = Global.Instance.CharacterInfoLookup[ evt.CharacterIdB ];
      /*
      // lazy lookup
      if( evt.CharacterA == null )
      {
        SerializedComponent sca = SerializedObject.ResolveComponentFromId( evt.CharacterIdA );
        if( sca != null )
          evt.CharacterA = (Character)sca;
      }
      // lazy lookup
      if( evt.CharacterB == null )
      {
        SerializedComponent scb = SerializedObject.ResolveComponentFromId( evt.CharacterIdB );
        if( scb != null )
          evt.CharacterB = (Character)scb;
      }
      */

      instigator.ReceiveEvent( evt );

      // 1) what happened. Whether I saw it, or someone told me. with correct object/subject dependent on who is listening, and pronouns, tense, etc.
      // 2) why it happened... assumptions, speculation, unknowns. 
      // 3) personal feelings, assessment, conclusion.

      string statement = "";
      string verb;
      switch( evt.type )
      {
        case CharacterEventEnum.Damage:
          verb = "attack";
          if( cia.id == id )
            statement += cib.name + " "+PastTense(verb)+" me " + ( Time.time - evt.time ) + " seconds ago.";
          else if( cib.id == id )
            statement += "I "+PastTense(verb)+" " + cia.name + " " + ( Time.time - evt.time ) + " seconds ago.";
          else if( witness.id == id )
            statement += "I saw "+ cib.name + " "+verb+" " + cia.name + " " + ( Time.time - evt.time ) + " seconds ago.";
          else if( witness.id == cia.id )
            statement += cia.name+" said "+cib.name + " "+PastTense(verb)+" " + ThirdPersonSingularObject(cia) + " " + ( Time.time - evt.time ) + " seconds ago.";
          else if( witness.id == cib.id )
            statement += cib.name+" said "+ThirdPersonSingularSubject(cib) + " "+PastTense(verb)+" " + cia.name + " " + ( Time.time - evt.time ) + " seconds ago.";
          else
            statement += witness.name +" saw "+ cib.name + " "+verb+" " + cia.name + " " + ( Time.time - evt.time ) + " seconds ago.";
          if( !cia.alive )
            statement += " "+Cap(cia.name) + " is dead.";
          break;

        case CharacterEventEnum.Death:
          statement += cib.name + " killed " + cia.name + "! "+ (Time.time-evt.time)+" seconds ago.";
          break;

        default:
          statement += evt.type.ToString() + " " + cia.name + " " + cib.name +" "+ (Time.time-evt.time).ToString("D")+" seconds ago.";
          statement += " " + ConstructStatementAbout( cib.id );
          break;
      }
      Speak( statement, 2 );

      /*
      // Gossip: pick a random affinity/character and say something about them
      string say = "...";
      List<int> keys = new List<int>();
      foreach( var pair in Affinity )
        if( pair.Key != instigator.id && pair.Key != GetInstanceID() )
          keys.Add( pair.Key );
      if( keys.Count > 0 )
      {
        Character cha = null;
        const int maxtries = 10;
        int tries = 0;
        while( cha == null )
        {
          tries++;
          if( tries > maxtries || tries > keys.Count )
            break;

          SerializedComponent sco = null;
          int key = keys[ GossipIndex++ % keys.Count ];
          if( SerializedObject.serializedComponents.ContainsKey( key ) )
            sco = SerializedObject.serializedComponents[ key ];
          else
            continue;
          if( sco == null )
          {
            // character is null. is dead?
            continue;
          }
          cha = (Character)sco;
          if( cha != null )
          {
            AffinityData ad = GetAffinity( cha );

            string ThirdPersonSingularSubject = "<ThirdPersonSingularSubject>";
            string ThirdPersonSingularObject = "<ThirdPersonSingularObject>";
            if( cha.identity.female )
            {
              ThirdPersonSingularSubject = "she";
              ThirdPersonSingularObject = "her";
            }
            else
            {
              ThirdPersonSingularSubject = "he";
              ThirdPersonSingularObject = "him";
            }

            if( ad.Value < AffPeaceful )
              say = "I hate " + cha.CharacterName + ". " + Cap( ThirdPersonSingularSubject ) + " can die.";
            else
              if( ad.Value < AffTalk )
                say = cha.CharacterName + " sucks. I will not talk to " + ThirdPersonSingularObject + ".";
              else
                if( ad.Value < 0 )
                  say = "I think " + cha.CharacterName + " needs to help out around here.";
                else
                  if( ad.Value < AffFollow )
                    say = cha.CharacterName + " seems okay.";
                  else
                    if( ad.Value < AffShare )
                      say = "I trust " + cha.CharacterName + ". I would follow " + ThirdPersonSingularObject + " if " + ThirdPersonSingularSubject + " asked.";
                    else
                      if( ad.Value < AffLoyal )
                        say = cha.CharacterName + " is great!";
                      else
                        say = "I love " + cha.CharacterName + ". " + Cap( ThirdPersonSingularSubject ) + " is the best.";
            break;
          }
        }
        Speak( say, 2 );

      }
      */
    }
  }

  string PastTense( string verb )
  {
    return verb + "ed";
  }

  string ThirdPersonSingularSubject( CharacterInfo cpi )
  {
    if( cpi.female )
      return "she";
    return "he";
  }

  string ThirdPersonSingularObject( CharacterInfo cpi )
  {
    if( cpi.female )
      return "her";
    return "him";
  }

  string ConstructStatementAbout( int characterId )
  {
    string say = "";
    CharacterInfo cia = Global.Instance.CharacterInfoLookup[ characterId ];
    AffinityData ad = GetAffinity( characterId );

    /*string ThirdPersonSingularSubject = "they";
    string ThirdPersonSingularObject = "them";
    if( cia.female )
    {
      ThirdPersonSingularSubject = "she";
      ThirdPersonSingularObject = "her";
    }
    else
    {
      ThirdPersonSingularSubject = "he";
      ThirdPersonSingularObject = "him";
    }
    if( ad.Value < AffPeaceful )
      say = "I hate " + cia.name + ". " + Cap( ThirdPersonSingularSubject ) + " can die.";
    else
      if( ad.Value < AffTalk )
        say = cia.name + " sucks. I will not talk to " + ThirdPersonSingularObject + ".";
      else
        if( ad.Value < 0 )
          say = "I think " + cia.name + " needs to help out around here.";
        else
          if( ad.Value < AffFollow )
            say = cia.name + " seems okay.";
          else
            if( ad.Value < AffShare )
              say = "I trust " + cia.name + ". I would follow " + ThirdPersonSingularObject + " if " + ThirdPersonSingularSubject + " asked.";
            else
              if( ad.Value < AffLoyal )
                say = cia.name + " is great!";
              else
                say = "I love " + cia.name + ". " + Cap( ThirdPersonSingularSubject ) + " is the best.";*/
    return say;
  }
#endif
}