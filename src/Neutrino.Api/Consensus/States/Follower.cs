using System;
using System.Threading;
using System.Threading.Tasks;
using Neutrino.Api.Consensus.Events;
using Neutrino.Api.Consensus.Responses;

namespace Neutrino.Api.Consensus.States
{
    public class Follower : State
    {
        private int _lastRertievedHeartbeat = 0;
        private CancellationTokenSource _checkHeartbeatTokenSource;
        private readonly IConsensusContext _consensusContext;

        public Follower(IConsensusContext consensusContext)
        {
            _consensusContext = consensusContext;
        }

        public override void Proceed()
        {
            StartCheckingHeartbeats();
        }

        public override IResponse TriggerEvent(IEvent triggeredEvent)
        {
            var heartbeatEvent = triggeredEvent as HeartbeatEvent;
            if(heartbeatEvent != null)
            {
                _consensusContext.LeaderNode = heartbeatEvent.Node;
                _consensusContext.CurrentTerm = heartbeatEvent.CurrentTerm;
                _lastRertievedHeartbeat = 0;
                return new EmptyResponse();
            }

            var leaderRequestEvent = triggeredEvent as LeaderRequestEvent;
            if(leaderRequestEvent != null)
            {
                bool voteValue = OtherNodeCanBeLeader(leaderRequestEvent);
                if(voteValue)
                {
                    _consensusContext.LeaderNode = leaderRequestEvent.Node;
                    _consensusContext.CurrentTerm = leaderRequestEvent.CurrentTerm;
                    _lastRertievedHeartbeat = 0;
                }

                return new VoteResponse(voteValue, _consensusContext.CurrentTerm, _consensusContext.CurrentNode);
            }

            return new EmptyResponse();
        }

        public override void DisposeCore()
        {
            _checkHeartbeatTokenSource.Cancel();
        }

        private void StartCheckingHeartbeats()
        {
            _checkHeartbeatTokenSource = new CancellationTokenSource();
            Task.Run(() => CheckHeartbeat(_checkHeartbeatTokenSource.Token), _checkHeartbeatTokenSource.Token);
        }

        private void StopCheckingHeartbeat()
        {
            _checkHeartbeatTokenSource.Cancel();
        }

        private void CheckHeartbeat(CancellationToken token)
        {
            while(!token.IsCancellationRequested) 
            {
                if(_lastRertievedHeartbeat >= _consensusContext.ElectionTimeout)
                {
                    StopCheckingHeartbeat();
                    _consensusContext.State = new Candidate(_consensusContext);
                }

                Thread.Sleep(50);
                _lastRertievedHeartbeat += 50;
            }
        }

        private bool OtherNodeCanBeLeader(LeaderRequestEvent leaderRequestEvent)
        {
            return leaderRequestEvent.CurrentTerm >= _consensusContext.CurrentTerm;
        }
    }
}