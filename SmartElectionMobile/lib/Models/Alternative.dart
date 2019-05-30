class Alternative{
  int id;
  String name;
  String info;
  int electionId;

  Alternative.fromJson(Map<String, dynamic> json)
      : id = json['id'],
        name = json['name'],
        electionId = json['electionId'],
        info = json['info'];
}