class Election {
  int id;
  String start;
  String end;

  Election.fromJson(Map<String, dynamic> json)
      : id = json['id'],
        start = json['start'],
        end = json['end'];
}