import 'package:flutter/material.dart';
import 'package:vote_system/Providers/httpProvider.dart';
import 'package:vote_system/Providers/userProvider.dart';
import 'dart:async';
import 'dart:convert' as JSON;

class LoginPage extends StatelessWidget {
  static GlobalKey<FormState> _formKey = GlobalKey<FormState>();
  final _usernameController = TextEditingController();
  final _passwordController = TextEditingController();

  final _loginURL = '/login';

  @override
  Widget build(BuildContext context) {
    var emailTextField = TextFormField(
      controller: _usernameController,
      keyboardType: TextInputType.emailAddress,
      decoration: const InputDecoration(labelText: 'Login:'),
      validator: (String value) {
        var isValid = value.isNotEmpty;
        return isValid ? "" : "Please, enter your email.";
      },
    );

    var passwordTextField = TextFormField(
      controller: _passwordController,
      obscureText: true,
      decoration: const InputDecoration(
        hintText: 'Please, enter your password.',
        filled: false,
        labelText: 'Password:',
      ),
      onSaved: (String value) {},
      validator: (String value) {
        var isValid = value.isNotEmpty;
        return isValid ? "" : "Please, enter your password.";
      },
    );

    var loginButton = RaisedButton(
      onPressed: () async{
        var isUserLoggedIn = await _performLogin();
        if(isUserLoggedIn) {
          print(isUserLoggedIn);
          Navigator.of(context)
              .pushReplacementNamed('signup/choose_credentials');
        }
      },
      child: Text('Log in'),
    );

    return new Scaffold(
        appBar: AppBar(
          title: const Text('Login'),
          backgroundColor: Colors.teal[400],
        ),
        body: (Align(
            alignment: Alignment.center,
            child: (Form(
              child: Column(
                key: _formKey,
                crossAxisAlignment: CrossAxisAlignment.center,
                mainAxisAlignment: MainAxisAlignment.center,
                children: <Widget>[
                  Padding(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 16.0, vertical: 8),
                      child: (emailTextField)),
                  Padding(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 16.0, vertical: 8),
                      child: (passwordTextField)),
                  Padding(
                    padding: const EdgeInsets.symmetric(vertical: 16.0),
                    child: loginButton,
                  ),
                ],
              ),
            )))));
  }

  Future<bool> _performLogin() async {
    String username = _usernameController.text;
    String password = _passwordController.text;

    var map = {
      'email': username,
      'password': password
    };

    var response = await HttpProvider.PostQuery(_loginURL, map);
    print(response);
    if(response['isSuccess']){
      dynamic reply = response['reply'];
      print(reply);
      var userProvider = new UserProvider();
      userProvider.userId = reply;
      await userProvider.GetElections();

    }

    return response['isSuccess'];
  }
}