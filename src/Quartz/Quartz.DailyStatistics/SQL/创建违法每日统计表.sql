declare  tableExist number;
begin
    select count(1) into tableExist from user_tables where upper(table_name)=upper('PUNISH_ILLEGALVEHICLECOUNT') ;
    if tableExist = 0  then

        execute immediate '

  CREATE TABLE PUNISH_ILLEGALVEHICLECOUNT
  (  
    SGUID VARCHAR2(50), 
    OCCERDATE VARCHAR2(50),    
    SPOTTINGID VARCHAR2(50),
    LEGALIZEILLEGALTYPENO VARCHAR2(50),
    ILLEGALTYPENO VARCHAR2(50),
    COUNT number,
    CREATEDTIME date,
    REMARK VARCHAR2(50),
    primary key (SGUID)
  )
  ';
        execute immediate 'comment ON TABLE  PUNISH_ILLEGALVEHICLECOUNT IS ''违法每日统计表''';
        execute immediate 'comment on column PUNISH_ILLEGALVEHICLECOUNT.SGUID is ''主键''';     
        execute immediate 'comment on column PUNISH_ILLEGALVEHICLECOUNT.OCCERDATE is ''违法日期''';
        execute immediate 'comment on column PUNISH_ILLEGALVEHICLECOUNT.SPOTTINGID is ''路口ID''';  
        execute immediate 'comment on column PUNISH_ILLEGALVEHICLECOUNT.LEGALIZEILLEGALTYPENO is ''标准代码''';     
        execute immediate 'comment on column PUNISH_ILLEGALVEHICLECOUNT.ILLEGALTYPENO is ''违法编号''';     
        execute immediate 'comment on column PUNISH_ILLEGALVEHICLECOUNT.CREATEDTIME is ''创建日期''';   
        execute immediate 'comment on column PUNISH_ILLEGALVEHICLECOUNT.REMARK is ''备注''';      
        execute immediate 'comment on column PUNISH_ILLEGALVEHICLECOUNT.COUNT is ''数量'''; 
        execute immediate 'create index INX_P_IC_OCCERDATE on PUNISH_ILLEGALVEHICLECOUNT(OCCERDATE)';
    end if;
end;
/

INSERT INTO SYS_DB_VERSION(UPDATEDID,APVERSION,SQLDESCRIPTION,UPDATEDTIME) 
VALUES(APP_DB_VERSION_ID_SEQ.NEXTVAL,'201706028.248','创建违法每日统计表',SYSDATE);
commit;