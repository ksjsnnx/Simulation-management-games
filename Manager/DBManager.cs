using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine;
/// <summary>
/// 数据库管理器   连接数据库的方法 
/// </summary>
public class DBManager : UnitySingleTonMono<DBManager>
{
    //连接字符串  告诉数据库的信息  服务器地址  数据库的名字  数据库的用户名 密码   编码格式  
    private string ConnectionString;//连接字符串 
    private MySqlConnection conn;
    private string severIp="127.0.0.1"; //数据库地址
    private string database = "mygamedb"; //数据库地址
    private string username="root";
    private string password="abcd1234";

    public override void Awake()
    {
        base.Awake();
        Init();
    }

    public void Init()
    {
        //创建这样的连接字符串  
        ConnectionString = $"Server={severIp};Database={database};Uid={username};Pwd={password};charset=utf8";
        //测试链接 
      
    }
    /// <summary>
    /// 创建数据库连接 
    /// </summary>
    public bool OpenConnection()
    {
        try
        {
            conn=new MySqlConnection(ConnectionString);//创建连接实例 
            conn.Open(); //打开数据库连接通道 3306   
            return true;
        }
        catch (MySqlException ex)
        {
            print(ex.Message);
            return false;
        }
    }
/// <summary>
/// 关闭连接
/// </summary>
/// <returns></returns>
    public bool CloseConnection()
    {
        if (conn!=null)
        {
            try
            {
                //关闭连接 
                conn.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                print(ex.Message);
                return false;
                
            }
        }
        else//当前没有连接
        {
            return false;
        }
    }
/// <summary>
/// 查询语句的方法
/// </summary>
/// <param name="query">查询语句</param>
/// <returns></returns>
    public DataTable SelectQuery(string query)
    {
        var dt=new DataTable();
        var cmd=new MySqlCommand(query,conn);
        var reader=cmd.ExecuteReader();
        dt.Load(reader);
        return dt;//行和列的数据结构   dt[行][列]
    }
/// <summary>
/// 非查询语句  
/// </summary>
/// <param name="query"></param>
    public void NonQuery(string query)
    {
        var cmd=new MySqlCommand(query,conn);
        cmd.ExecuteNonQuery();
    }

}
