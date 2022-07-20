using Caliburn.Micro;
using Dapper;
using Ironwall.Framework.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class VcaDomainDataProvider
        : TaskService, IDataProviderService
    {
        #region - Ctors -
        public VcaDomainDataProvider(
            VcaProvider vcaProvider
            , PresetProvider presetProvider
            , VcaSetupModel setupModel
            , IEventAggregator eventAggregator
            , IDbConnection dbConnection)
        {
            _vcaProvider = vcaProvider;
            _presetProivder = presetProvider;
            _eventAggregator = eventAggregator;
            _dbConnection = dbConnection;
            _setupModel = setupModel;
        }


        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task RunTask(CancellationToken token = default)
        {
            await Task.Run(delegate { BuildSchemeAsync(); })
                          .ContinueWith(delegate { FetchAsync(); }, TaskContinuationOptions.ExecuteSynchronously, token);
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private async void BuildSchemeAsync()
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                using var cmd = _dbConnection.CreateCommand();
                var tableVca = _setupModel.TableVca;
                cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {tableVca} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            typedevice INTEGER,
                                            namedevice TEXT,
                                            serialdevice INTEGER,    
                                            channel INTEGER,
                                            status INTEGER,
                                            used INTEGER,
                                            time_created DATETIME DEFAULT CURRENT_TIMESTAMP NOT NULL
                                           )";
                cmd.ExecuteNonQuery();

                tableVca = _setupModel.TablePreset;
                cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {tableVca} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            namearea TEXT,
                                            namedevice TEXT,
                                            home INTEGER,    
                                            preset INTEGER,
                                            controltime INTEGER,
                                            used INTEGER,
                                            time_created DATETIME DEFAULT CURRENT_TIMESTAMP NOT NULL
                                           )";
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                _dbConnection.Close();
                Debug.WriteLine($"PrepareConfigure: {ex.Message}");
            }
        }

        private async void FetchAsync()
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                /////////////////////////////////////////////////////////////////////
                ////                           Vnc
                /////////////////////////////////////////////////////////////////////
                var tableVca = _setupModel.TableVca;
                var sql = @$"SELECT * FROM {tableVca} WHERE used = '1'";
                foreach (var vcaModel in (_dbConnection
                    .Query<VcaModel>(sql)
                    .Select((item) => new VcaViewModel(item))))
                {
                    _vcaProvider.Add(vcaModel);
                };

                /////////////////////////////////////////////////////////////////////
                ////                           Preset
                /////////////////////////////////////////////////////////////////////
                var tablePreset = _setupModel.TablePreset;
                sql = @$"SELECT * FROM {tablePreset} WHERE used = '1'";
                foreach (var presetModel in (_dbConnection
                    .Query<PresetModel>(sql)
                    .Select((item) => new PresetViewModel(item))))
                {
                    _presetProivder.Add(presetModel);
                };

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FetchAsync: {ex.Message}");
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        private VcaProvider _vcaProvider { get; set; }
        private PresetProvider _presetProivder { get; set; }
        #endregion
        #region - Attributes -
        private IEventAggregator _eventAggregator;
        private IDbConnection _dbConnection;
        private VcaSetupModel _setupModel;
        #endregion
    }
}
