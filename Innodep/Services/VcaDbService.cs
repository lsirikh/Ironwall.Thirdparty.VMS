using Caliburn.Micro;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class VcaDbService
    {

        #region - Ctors -
        public VcaDbService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnPublishedThread(this);

            DbConnection = IoC.Get<IDbConnection>();

            DeviceProvider = IoC.Get<VcaProvider>();
            PresetProvider = IoC.Get<PresetProvider>();
            SetupModel = IoC.Get<VcaSetupModel>();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async Task SaveDevice()
        {
            int commitResult = 0;
            int commitCount = 0;

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var conn = DbConnection as SQLiteConnection;
                    var table = SetupModel.TableVca;

                    var dbProvider = new VcaProvider();

                    //최신 DB provider
                    var sql = @$"SELECT * FROM {table} WHERE used = '1'";
                    foreach (var vcaModel in (DbConnection
                        .Query<VcaModel>(sql)
                        .Select((item) => new VcaViewModel(item))))
                    {
                        dbProvider.Add(vcaModel);
                    };

                    //최신 DB에는 있는데
                    //Provider에는 없다.
                    //DB 삭제인데.

                    foreach (var item in DeviceProvider.CollectionEntity)
                    {
                        //DB에 있고, Provider에도 있는 레코드
                        if (dbProvider.CollectionEntity.Where(entity => entity.Id == item.Id).Count() > 0)
                            commitResult = conn.Execute(@$"UPDATE {table} 
                                            SET typedevice=@TypeDevice, namedevice=@NameDevice, serialdevice=@SerialDevice
                                            , channel=@Channel, status=@Status, used=@Used WHERE id = @Id", item);

                        //DB에는 없고, Provider에 있는 추가된 레코드
                        else
                            commitResult = conn.Execute(@$"INSERT INTO {table} 
                                    (id, typedevice, namedevice, serialdevice, channel, status, used) VALUES 
                                    (@Id, @TypeDevice, @NameDevice, @SerialDevice, @Channel, @Status, @Used)", item);

                        commitCount += commitResult;
                    }

                    //DB에는 있고, Provider에 없는 레코드
                    foreach (var item in dbProvider.CollectionEntity)
                    {
                        if (DeviceProvider.CollectionEntity.Where(entity => entity.Id == item.Id).Count() > 0)
                            continue;
                        else
                            commitResult = conn.Execute(@$"DELETE FROM {table} WHERE id = @Id", item);
                    }

                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine("Theading to insert DB data Error: " + ex.Message);
                }
            });


        }

        public async Task FetchDevice()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var conn = DbConnection as SQLiteConnection;
                    var table = SetupModel.TableVca;

                    DeviceProvider.Clear();

                    var sql = @$"SELECT * FROM {table} WHERE used = '1'";
                    foreach (var vcaModel in (DbConnection
                        .Query<VcaModel>(sql)
                        .Select((item) => new VcaViewModel(item))))
                    {
                        DeviceProvider.Add(vcaModel);
                    };
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine("Theading to insert DB data Error: " + ex.Message);
                }
            });
        }

        public async Task SavePreset()
        {
            int commitResult = 0;
            int commitCount = 0;

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var conn = DbConnection as SQLiteConnection;
                    var table = SetupModel.TablePreset;

                    var dbProvider = new PresetProvider();

                    //최신 DB provider
                    var sql = @$"SELECT * FROM {table} WHERE used = '1'";
                    foreach (var model in (DbConnection
                        .Query<PresetModel>(sql)
                        .Select((item) => new PresetViewModel(item))))
                    {
                        dbProvider.Add(model);
                    };

                    //최신 DB에는 있는데
                    //Provider에는 없다.
                    //DB 삭제인데.

                    foreach (var item in PresetProvider.CollectionEntity)
                    {
                        //DB에 있고, Provider에도 있는 레코드
                        if (dbProvider.CollectionEntity.Where(entity => entity.Id == item.Id).Count() > 0)
                            commitResult = conn.Execute(@$"UPDATE {table} 
                                            SET namearea=@NameArea, namedevice=@NameDevice,
                                            home=@Home, preset=@Preset, controltime=@ControlTime, used=@Used 
                                            WHERE id = @Id", item);

                        //DB에는 없고, Provider에 있는 추가된 레코드
                        else
                            commitResult = conn.Execute(@$"INSERT INTO {table} 
                                    (id, namearea, namedevice, home, preset, controltime, used) VALUES 
                                    (@Id, @NameArea, @NameDevice, @Home, @Preset, @ControlTime, @Used)", item);

                        commitCount += commitResult;
                    }

                    //DB에는 있고, Provider에 없는 레코드
                    foreach (var item in dbProvider.CollectionEntity)
                    {
                        if (PresetProvider.CollectionEntity.Where(entity => entity.Id == item.Id).Count() > 0)
                            continue;
                        else
                            commitResult = conn.Execute(@$"DELETE FROM {table} WHERE id = @Id", item);
                    }

                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine("Theading to insert DB data Error: " + ex.Message);
                }
            });

        }

        public async Task FetchPreset()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var conn = DbConnection as SQLiteConnection;
                    var table = SetupModel.TablePreset;

                    PresetProvider.Clear();

                    var sql = @$"SELECT * FROM {table} WHERE used = '1'";
                    foreach (var model in (DbConnection
                        .Query<PresetModel>(sql)
                        .Select((item) => new PresetViewModel(item))))
                    {
                        PresetProvider.Add(model);
                    };
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine("Theading to insert DB data Error: " + ex.Message);
                }
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public VcaProvider DeviceProvider { get; private set; }
        public PresetProvider PresetProvider { get; private set; }
        public VcaSetupModel SetupModel { get; private set; }
        private IDbConnection DbConnection { get; set; }
        #endregion
        #region - Attributes -
        private IEventAggregator _eventAggregator;
        #endregion
    }
}
