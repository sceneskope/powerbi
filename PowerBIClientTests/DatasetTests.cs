using SceneSkope.PowerBI;
using SceneSkope.PowerBI.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PowerBIClientTests
{
    public class DatasetTests : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public DatasetTests(TestContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task TestListDatasetsAlsoCheckingBasicAuthentication()
        {
            var client = _context.CreateClient();
            var datasets = await client.ListAllDatasetsAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.NotNull(datasets);
        }

        [Fact(Skip="Groups not in permissions")]
        public async Task TestListGroups()
        {
            var client = _context.CreateClient();
            var groups = await client.ListAllGroupsAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.NotNull(groups);
        }

        [Fact(Skip = "Groups not in permissions")]
        public async Task TestCreateGroupDataset()
        {
            var client = _context.CreateClient();
            var groups = await client.ListAllGroupsAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.NotNull(groups);
            var dev = groups.First(g => g.Name == "Dev");
            Assert.NotNull(dev);
            var groupClient = client.CreateGroupClient(dev.Id);

            const string name = "devdummy";
            await DeleteDatasetAsync(groupClient, name).ConfigureAwait(false);

            var dataset = new Dataset
            {
                Name = name,
                DefaultMode = DatasetMode.PushStreaming,

                Tables = new[]
                  {
                      new Table
                      {
                           Name = "devdummytable",
                            Columns = new[]
                            {
                                new Column
                                {
                                     Name = "a",
                                      DataType = DataType.String
                                }
                            }
                      }
                  }
            };
            var id = await groupClient.CreateDatasetAsync(dataset, DefaultRetentionPolicy.basicFifo, CancellationToken.None).ConfigureAwait(false);
            Assert.NotNull(id);

            var listing = await groupClient.ListAllTablesAsync(id.Id, CancellationToken.None).ConfigureAwait(false);
            Assert.NotNull(listing);
        }

        [Fact]
        public async Task CreateDummyDataset()
        {
            var client = _context.CreateClient();
            const string name = "dummy";
            await DeleteDatasetAsync(client, name).ConfigureAwait(false);

            var dataset = new Dataset
            {
                Name = name,
                DefaultMode = DatasetMode.PushStreaming,

                Tables = new[]
                  {
                      new Table
                      {
                           Name = "dummytable",
                            Columns = new[]
                            {
                                new Column
                                {
                                     Name = "a",
                                      DataType = DataType.String
                                }
                            }
                      }
                  }
            };
            await client.CreateDatasetAsync(dataset, DefaultRetentionPolicy.basicFifo, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateDummyDatasetAndAddData()
        {
            var client = _context.CreateClient();
            const string name = "dummyWithData";
            await DeleteDatasetAsync(client, name).ConfigureAwait(false);

            var dataset = new Dataset
            {
                Name = name,
                DefaultMode = DatasetMode.PushStreaming,

                Tables = new[]
                  {
                      new Table
                      {
                           Name = "dummytable",
                            Columns = new[]
                            {
                                new Column
                                {
                                     Name = "a",
                                      DataType = DataType.String
                                }
                            }
                      }
                  }
            };
            var datasetId = await client.CreateDatasetAsync(dataset, DefaultRetentionPolicy.basicFifo, CancellationToken.None).ConfigureAwait(false);

            var rows = new[]
            {
                new { a = "row1" },
                new { a = "row2" }
            };

            await client.AddRowsAsync(datasetId.Id, dataset.Tables[0].Name, rows, CancellationToken.None).ConfigureAwait(false);
            await client.ClearRowsAsync(datasetId.Id, dataset.Tables[0].Name, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task CheckGetSequenceNumbers()
        {
            var client = _context.CreateClient();
            const string name = "dummyWithData";
            await DeleteDatasetAsync(client, name).ConfigureAwait(false);

            var dataset = new Dataset
            {
                Name = name,
                DefaultMode = DatasetMode.PushStreaming,

                Tables = new[]
                  {
                      new Table
                      {
                           Name = "dummytable",
                            Columns = new[]
                            {
                                new Column
                                {
                                     Name = "a",
                                      DataType = DataType.String
                                }
                            }
                      }
                  }
            };
            var datasetId = await client.CreateDatasetAsync(dataset, DefaultRetentionPolicy.basicFifo, CancellationToken.None).ConfigureAwait(false);
            Assert.NotNull(datasetId);

            var sequenceNumbers = await client.GetTableSequenceNumbersAsync(datasetId.Id, dataset.Tables[0].Name, CancellationToken.None).ConfigureAwait(false);
            Assert.NotNull(sequenceNumbers);
            Assert.Equal(0, sequenceNumbers.Length);

            var rows = new[]
            {
                new { a = "row1" },
                new { a = "row2" }
            };

            await client.AddRowsAsync(datasetId.Id, dataset.Tables[0].Name, rows, CancellationToken.None).ConfigureAwait(false);
            sequenceNumbers = await client.GetTableSequenceNumbersAsync(datasetId.Id, dataset.Tables[0].Name, CancellationToken.None).ConfigureAwait(false);
            Assert.NotNull(sequenceNumbers);
            Assert.Equal(0, sequenceNumbers.Length);

            await client.AddRowsAsync(datasetId.Id, dataset.Tables[0].Name, rows, 2, CancellationToken.None).ConfigureAwait(false);
            sequenceNumbers = await client.GetTableSequenceNumbersAsync(datasetId.Id, dataset.Tables[0].Name, CancellationToken.None).ConfigureAwait(false);
            Assert.NotNull(sequenceNumbers);
            Assert.Equal(1, sequenceNumbers.Length);
            Assert.Equal(2, sequenceNumbers[0].SequenceNumber);

            await client.ClearRowsAsync(datasetId.Id, dataset.Tables[0].Name, CancellationToken.None).ConfigureAwait(false);
            sequenceNumbers = await client.GetTableSequenceNumbersAsync(datasetId.Id, dataset.Tables[0].Name, CancellationToken.None).ConfigureAwait(false);
            Assert.NotNull(sequenceNumbers);
        }

        private static async Task DeleteDatasetAsync(PowerBIClient client, string name)
        {
            var datasets = await client.ListAllDatasetsAsync(CancellationToken.None).ConfigureAwait(false);
            var dataset = Array.Find(datasets, d => d.Name == name);
            if (dataset != null)
            {
                await client.DeleteDatasetAsync(dataset.Id, CancellationToken.None).ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task UpdateDummyDataset()
        {
            var ct = CancellationToken.None;
            var client = _context.CreateClient();
            const string name = "test1";
            await DeleteDatasetAsync(client, name).ConfigureAwait(false);
            var dataset = new Dataset
            {
                Name = name,
                DefaultMode = DatasetMode.PushStreaming,

                Tables = new[]
                  {
                      new Table
                      {
                           Name = "dummytable",
                            Columns = new[]
                            {
                                new Column
                                {
                                     Name = "a",
                                      DataType = DataType.String
                                }
                            }
                      }
                  }
            };
            var datasetId = await client.CreateDatasetAsync(dataset, DefaultRetentionPolicy.basicFifo, ct).ConfigureAwait(false);

            var allDatasets = await client.ListAllDatasetsAsync(ct).ConfigureAwait(false);
            var datasetWithId = allDatasets.SingleOrDefault(d => d.Id == datasetId.Id);
            Assert.NotNull(datasetWithId);

            var allTables = await client.ListAllTablesAsync(datasetId.Id, ct).ConfigureAwait(false);

            var updatedTable = new Table
            {
                Name = "dummytable",
                Columns = new[]
                {
                                new Column
                                {
                                     Name = "a",
                                      DataType = DataType.String
                                },
                                new Column
                                {
                                    Name = "b",
                                    DataType = DataType.String
                                }

                }
            };
            await client.UpdateDatasetTableAsync(datasetId.Id, updatedTable, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateMultipleTableDataset()
        {
            var client = _context.CreateClient();
            const string name = "multi1";
            await DeleteDatasetAsync(client, name).ConfigureAwait(false);

            var dataset = new Dataset
            {
                Name = name,
                DefaultMode = DatasetMode.PushStreaming,

                Tables = new[]
                  {
                      new Table
                      {
                           Name = "table1",
                            Columns = new[]
                            {
                                new Column
                                {
                                     Name = "a",
                                      DataType = DataType.String
                                }
                            }
                      },
                      new Table
                      {
                          Name = "table2",
                          Columns = new[]
                          {
                              new Column
                              {
                                  Name = "b",
                                  DataType = DataType.DateTime
                              }
                          }
                      }
                  }
            };
            await client.CreateDatasetAsync(dataset, DefaultRetentionPolicy.basicFifo, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
