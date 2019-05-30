import 'package:flutter/material.dart';
import 'package:vote_system/Providers/userProvider.dart';

class AlternativePage extends StatelessWidget {
  static GlobalKey<FormState> _formKey = GlobalKey<FormState>();
  final UserProvider user = new UserProvider();

  @override
  Widget build(BuildContext context) {
    var alternatives = user.alternatives.where((x) => x.electionId == user.elections[user.selectedElection].id).toList();

    var confirmButton = RaisedButton(
      onPressed: () async{
//        var isUserLoggedIn = await _performLogin();
//        if(isUserLoggedIn) {
//          print(isUserLoggedIn);
          Navigator.of(context)
              .pushReplacementNamed('signup/choose_credentials');
//        }
      },
      child: Text('Vote'),
    );

    var cancelButton = RaisedButton(
      onPressed: () async{
//        var isUserLoggedIn = await _performLogin();
//        if(isUserLoggedIn) {
//          print(isUserLoggedIn);
        Navigator.of(context)
            .pushReplacementNamed('signup/choose_credentials');
//        }
      },
      child: Text('Cancel'),
    );

    var alternativeList = ListView.builder(
        padding: const EdgeInsets.all(8.0),
        itemCount: alternatives.length,
        itemBuilder: (BuildContext context, int index) {
          return Container(
              height: 80,
              child:  Column(
                  children: <Widget>
                  [
                    new Row(
                      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                      children: <Widget>[
                        new Radio(
                          value: index,
                          groupValue: 'alternative',
                          onChanged: (var obj){},
                        ),
                        new Text(
                          '${alternatives[index].name}',
                          style: new TextStyle(fontSize: 16.0),
                        ),
                      ],
                    ),

                    new Text(
                      '${alternatives[index].info}',
                      style: new TextStyle(fontSize: 16.0),
                    ),
                  ]
              )
          );
        }
    );

    return new Scaffold(
        appBar: AppBar(
          title: const Text('Contracts'),
          backgroundColor: Colors.teal[400],
        ),
        body: (Align(
            alignment: Alignment.topCenter,
            child:(alternativeList))));
//    Column(
//        children: <Widget>
//        [
//          alternativeList,
//          new Row(
//            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
//            children: <Widget>[
//              confirmButton,
//              cancelButton],
//          )
//        ]
//    )
  }
}