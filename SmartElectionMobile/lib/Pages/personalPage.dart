import 'package:flutter/material.dart';
import 'package:vote_system/Providers/userProvider.dart';

class UserDocuments extends StatelessWidget {
  static GlobalKey<FormState> _formKey = GlobalKey<FormState>();
  final UserProvider user = new UserProvider();

  @override
  Widget build(BuildContext context) {

    var electionList = ListView.builder(
        padding: const EdgeInsets.all(8.0),
        itemCount: user.elections.length,
        itemBuilder: (BuildContext context, int index) {
          return Container(
              height: 80,
              child:  Column(
                  children: <Widget>
                  [
                    new Text(
                      'Election ${user.elections[index].id}',
                      style: new TextStyle(
                        fontSize: 24.0,
                        fontFamily: 'Roboto',
                      ),
                    ),

                    new Row(
                      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                      children: <Widget>[
                        new RaisedButton(
                          padding: const EdgeInsets.all(8.0),
                          textColor: Colors.white,
                          color: Colors.teal[400],

                          onPressed: (){
                            user.selectedElection = index;
                            Navigator.of(context)
                                .pushReplacementNamed('election/alternatives');
                          },
                          child: new Text("Vote"),
                        ),

                        new RaisedButton(
                          onPressed: (){
                            user.selectedElection = index;
                            Navigator.of(context)
                                .pushReplacementNamed('election/alternatives');
                          },
                          textColor: Colors.white,
                          color: Colors.teal[400],
                          padding: const EdgeInsets.all(8.0),
                          child: new Text("Result",),
                        ),
                      ],
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
            child:(electionList))));
  }
}