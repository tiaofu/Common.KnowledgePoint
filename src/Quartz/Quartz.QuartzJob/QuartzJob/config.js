{
    "V2":"DATA SOURCE=192.168.0.245:1521/tjims;PASSWORD=citms;PERSIST SECURITY INFO=True;USER ID=hsepp",
    "V4":"DATA SOURCE=192.168.0.245:1521/tjims;PASSWORD=citms;PERSIST SECURITY INFO=True;USER ID=hsepp",
    "ST":"2016-05-02 00:00:00",
    "ET":"2016-05-02 02:00:00",
    "DisplayName":"违法数据转换服务",
    "ServiceName":"CopyIllegalDataServer",
    "Range":"10",
    "Format":[
        {
            "Field":"PlateColor",
            "Values":["12|1","34|1","99|9"]
        },
        {
            "Field":"PlateType",
            "Values":["12|01"]
        },
        {
            "Field":"DepartmentId",
            "Values":[]
        }
    ],  
    "Sql":"select * from punish_illegalvehicle where \"Status\" = 60 and \"Timestamp\" > = to_date(:starttime,'yyyy/MM/dd HH24:mi:ss') and \"Timestamp\" < to_date(:endtime,'yyyy/MM/dd HH24:mi:ss')"
}