import 'package:vote_system/Models/Election.dart';
import 'package:vote_system/Models/Alternative.dart';
import 'package:vote_system/Providers/httpProvider.dart';
import 'dart:convert' as JSON;

class UserProvider{
  static final _instance = new UserProvider._internal();
  final _electionsURL = '/api/Elections';
  final _alternativesURL = '/api/Alternatives';

  List<Election> elections = new List<Election>();
  List<Alternative> alternatives = new List<Alternative>();
  int selectedElection;
  String userId;

  factory UserProvider(){
    return _instance;
  }

  Future GetElections() async{
    var response = await HttpProvider.GetQuery(_electionsURL);
    if(response['isSuccess']){
      print(response['reply']);
      var electionsFromJson = JSON.jsonDecode(response['reply']);

      for(var election in electionsFromJson) {
        elections.add(new Election.fromJson(election));
      }
    }
    await GetAlternatives();
  }

  Future GetAlternatives() async{
    var response = await HttpProvider.GetQuery(_alternativesURL);
    if(response['isSuccess']){
      print(response['reply']);
      var alternativesFromJson = JSON.jsonDecode(response['reply']);

      for(var alternative in alternativesFromJson) {
        var alternativeModel = new Alternative.fromJson(alternative);

        alternatives.add(alternativeModel);
      }
    }
  }

  UserProvider._internal();

}