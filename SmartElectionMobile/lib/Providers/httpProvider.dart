import 'dart:convert';
import 'dart:io';
import 'dart:async';

class HttpProvider
{
  static String routeServerUrl = 'https://437a269b.ngrok.io';

  static Future<Map> PostQuery(String URL, Map jsonMap) async{
    var httpClient = new HttpClient();
    print('before POST request $URL');
    var request = await httpClient.postUrl(Uri.parse(routeServerUrl + URL));
    print('POST request $URL is fulfilled.');
    request.headers.set('content-type', 'application/json');
    request.add(utf8.encode(json.encode(jsonMap)));
    HttpClientResponse response = await request.close();

    var isRequestSuccessful = isResponseSuccessful(response.statusCode);
    var reply = await response.transform(utf8.decoder).join();
    print('POST request $URL result: $reply');

    var map = {
      'isSuccess': isRequestSuccessful,
      'reply': reply
    };
    httpClient.close();

    return map;
  }

  static Future<Map> GetQuery(String URL) async{
    var httpClient = new HttpClient();

    var request = await HttpClient().getUrl(Uri.parse(routeServerUrl + URL));
    var response = await request.close();

    var isRequestSuccessful = isResponseSuccessful(response.statusCode);
    var reply = await response.transform(utf8.decoder).join();
    httpClient.close();

    var map = {
      'isSuccess': isRequestSuccessful,
      'reply': reply
    };
    return map;
  }

  static bool isResponseSuccessful(int statusCode){
    return statusCode.toString().startsWith('2');
  }
}